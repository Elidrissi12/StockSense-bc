using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Enums;

namespace StockManagement.Application.Services;

public class ReportService : IReportService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPdfReportGenerator _pdfReportGenerator;
    private readonly IExcelReportGenerator _excelReportGenerator;
    private readonly IEmailService _emailService;

    public ReportService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IStockMovementRepository stockMovementRepository,
        IFileStorageService fileStorageService,
        IPdfReportGenerator pdfReportGenerator,
        IExcelReportGenerator excelReportGenerator,
        IEmailService emailService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _stockMovementRepository = stockMovementRepository;
        _fileStorageService = fileStorageService;
        _pdfReportGenerator = pdfReportGenerator;
        _excelReportGenerator = excelReportGenerator;
        _emailService = emailService;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int recentDays = 7)
    {
        var products = await _productRepository.GetAllAsync();
        var fromUtc = DateTime.UtcNow.Date.AddDays(-Math.Max(1, recentDays));
        var toUtc = DateTime.UtcNow;
        var recentMovements = await _stockMovementRepository.GetInPeriodAsync(fromUtc, toUtc);

        return new DashboardSummaryDto(
            TotalProducts: products.Count,
            CriticalProducts: products.Count(p => p.Quantite < p.SeuilMin),
            TotalStockQuantity: products.Sum(p => p.Quantite),
            RecentMovementsCount: recentMovements.Count);
    }

    public async Task<IReadOnlyList<CategoryStockStatDto>> GetCategoryStockStatsAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();

        var result = categories
            .Select(c =>
            {
                var inCategory = products.Where(p => p.CategorieId == c.Id).ToArray();
                return new CategoryStockStatDto(
                    c.Id,
                    c.Name,
                    inCategory.Length,
                    inCategory.Sum(p => p.Quantite));
            })
            .OrderByDescending(x => x.TotalQuantity)
            .ToArray();

        return result;
    }

    public async Task<IReadOnlyList<StockTrendPointDto>> GetStockTrendAsync(int days = 30)
    {
        var safeDays = Math.Clamp(days, 1, 365);
        var fromUtc = DateTime.UtcNow.Date.AddDays(-safeDays + 1);
        var toUtc = DateTime.UtcNow;
        var movements = await _stockMovementRepository.GetInPeriodAsync(fromUtc, toUtc);

        var points = Enumerable.Range(0, safeDays)
            .Select(offset => fromUtc.AddDays(offset))
            .Select(day =>
            {
                var dayMovements = movements.Where(m => m.DateUtc.Date == day.Date);
                var entries = dayMovements
                    .Where(m => m.Type == StockMovementType.Entree)
                    .Sum(m => m.Quantite);
                var exits = dayMovements
                    .Where(m => m.Type == StockMovementType.Sortie)
                    .Sum(m => m.Quantite);

                return new StockTrendPointDto(
                    DateUtc: day,
                    TotalEntries: entries,
                    TotalExits: exits,
                    NetMovement: entries - exits);
            })
            .ToArray();

        return points;
    }

    public async Task<IReadOnlyList<RecentMovementDto>> GetRecentMovementsAsync(int limit = 20)
    {
        var safeLimit = Math.Clamp(limit, 1, 100);
        var movements = await _stockMovementRepository.GetRecentAsync(safeLimit);
        var products = await _productRepository.GetAllAsync();
        var productsById = products.ToDictionary(p => p.Id);

        return movements.Select(m =>
        {
            productsById.TryGetValue(m.ProductId, out var p);
            return new RecentMovementDto(
                MovementId: m.Id,
                ProductId: m.ProductId,
                ProductName: p?.Nom ?? "Produit inconnu",
                ProductReference: p?.Reference ?? string.Empty,
                Type: m.Type,
                Quantite: m.Quantite,
                DateUtc: m.DateUtc,
                UserId: m.UserId);
        }).ToArray();
    }

    public async Task<GeneratedReportDto> ExportPdfAsync(ReportRequestDto request, Guid? userId)
    {
        var (summary, categoryStats, trend, recent) = await BuildReportDataAsync(request);
        var generated = await _pdfReportGenerator.GenerateStockReportAsync(summary, categoryStats, trend, recent);

        var stored = await _fileStorageService.SaveAsync(new StoredFileCreateRequest(
            generated.FileName,
            generated.ContentType,
            generated.Data,
            "report-pdf",
            userId));

        return new GeneratedReportDto(
            stored.Id,
            stored.FileName,
            stored.ContentType,
            stored.SizeBytes,
            stored.CreatedAtUtc,
            stored.Tag);
    }

    public async Task<GeneratedReportDto> ExportExcelAsync(ReportRequestDto request, Guid? userId)
    {
        var (summary, categoryStats, trend, recent) = await BuildReportDataAsync(request);
        var generated = await _excelReportGenerator.GenerateStockReportAsync(summary, categoryStats, trend, recent);

        var stored = await _fileStorageService.SaveAsync(new StoredFileCreateRequest(
            generated.FileName,
            generated.ContentType,
            generated.Data,
            "report-excel",
            userId));

        return new GeneratedReportDto(
            stored.Id,
            stored.FileName,
            stored.ContentType,
            stored.SizeBytes,
            stored.CreatedAtUtc,
            stored.Tag);
    }

    public async Task<GeneratedReportDto> SendReportByEmailAsync(EmailReportRequestDto request, Guid? userId)
    {
        var format = request.Format?.Trim().ToLowerInvariant();
        var exportRequest = new ReportRequestDto(request.FromUtc, request.ToUtc, 20);

        GeneratedReportDto generated = format switch
        {
            "pdf" => await ExportPdfAsync(exportRequest, userId),
            "excel" => await ExportExcelAsync(exportRequest, userId),
            "xlsx" => await ExportExcelAsync(exportRequest, userId),
            _ => throw new InvalidOperationException("Format de rapport non supporté. Utiliser 'pdf' ou 'excel'.")
        };

        var file = await _fileStorageService.DownloadAsync(generated.FileId)
                   ?? throw new InvalidOperationException("Impossible de télécharger le rapport généré.");

        await _emailService.SendWithAttachmentAsync(
            request.ToEmail,
            request.Subject,
            request.Body,
            file.FileName,
            file.ContentType,
            file.Data);

        return generated;
    }

    private async Task<(DashboardSummaryDto Summary, IReadOnlyList<CategoryStockStatDto> CategoryStats, IReadOnlyList<StockTrendPointDto> Trend, IReadOnlyList<RecentMovementDto> Recent)> BuildReportDataAsync(ReportRequestDto request)
    {
        var from = request.FromUtc ?? DateTime.UtcNow.Date.AddDays(-30);
        var to = request.ToUtc ?? DateTime.UtcNow;
        if (to < from)
            throw new InvalidOperationException("La période est invalide.");

        var safeLimit = Math.Clamp(request.Limit <= 0 ? 20 : request.Limit, 1, 100);

        var products = await _productRepository.GetAllAsync();
        var movements = await _stockMovementRepository.GetInPeriodAsync(from, to);
        var categories = await _categoryRepository.GetAllAsync();

        var summary = new DashboardSummaryDto(
            TotalProducts: products.Count,
            CriticalProducts: products.Count(p => p.Quantite < p.SeuilMin),
            TotalStockQuantity: products.Sum(p => p.Quantite),
            RecentMovementsCount: movements.Count);

        var categoryStats = categories
            .Select(c =>
            {
                var inCategory = products.Where(p => p.CategorieId == c.Id).ToArray();
                return new CategoryStockStatDto(c.Id, c.Name, inCategory.Length, inCategory.Sum(p => p.Quantite));
            })
            .OrderByDescending(x => x.TotalQuantity)
            .ToArray();

        var totalDays = Math.Clamp((to.Date - from.Date).Days + 1, 1, 365);
        var trend = Enumerable.Range(0, totalDays)
            .Select(offset => from.Date.AddDays(offset))
            .Select(day =>
            {
                var dayMovements = movements.Where(m => m.DateUtc.Date == day);
                var entries = dayMovements.Where(m => m.Type == StockMovementType.Entree).Sum(m => m.Quantite);
                var exits = dayMovements.Where(m => m.Type == StockMovementType.Sortie).Sum(m => m.Quantite);
                return new StockTrendPointDto(day, entries, exits, entries - exits);
            })
            .ToArray();

        var productsById = products.ToDictionary(p => p.Id);
        var recent = movements
            .OrderByDescending(m => m.DateUtc)
            .Take(safeLimit)
            .Select(m =>
            {
                productsById.TryGetValue(m.ProductId, out var p);
                return new RecentMovementDto(
                    m.Id,
                    m.ProductId,
                    p?.Nom ?? "Produit inconnu",
                    p?.Reference ?? string.Empty,
                    m.Type,
                    m.Quantite,
                    m.DateUtc,
                    m.UserId);
            })
            .ToArray();

        return (summary, categoryStats, trend, recent);
    }
}

