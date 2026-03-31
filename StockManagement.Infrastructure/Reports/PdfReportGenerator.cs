using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;

namespace StockManagement.Infrastructure.Reports;

public class PdfReportGenerator : IPdfReportGenerator
{
    private static bool _licenseConfigured;
    private static readonly object Sync = new();

    public Task<(byte[] Data, string FileName, string ContentType)> GenerateStockReportAsync(
        DashboardSummaryDto summary,
        IReadOnlyList<CategoryStockStatDto> categoryStats,
        IReadOnlyList<StockTrendPointDto> trend,
        IReadOnlyList<RecentMovementDto> recentMovements)
    {
        EnsureLicense();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text($"Stock Report - {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").SemiBold().FontSize(18);
                page.Content().Column(column =>
                {
                    column.Spacing(8);

                    column.Item().Text("Dashboard").Bold().FontSize(14);
                    column.Item().Text($"Total produits: {summary.TotalProducts}");
                    column.Item().Text($"Produits critiques: {summary.CriticalProducts}");
                    column.Item().Text($"Quantité totale: {summary.TotalStockQuantity}");
                    column.Item().Text($"Mouvements récents: {summary.RecentMovementsCount}");

                    column.Item().PaddingTop(8).Text("Stats par catégorie").Bold().FontSize(14);
                    foreach (var stat in categoryStats.Take(10))
                    {
                        column.Item().Text($"{stat.CategoryName}: {stat.ProductCount} produits, quantité {stat.TotalQuantity}");
                    }

                    column.Item().PaddingTop(8).Text("Derniers mouvements").Bold().FontSize(14);
                    foreach (var movement in recentMovements.Take(15))
                    {
                        column.Item().Text(
                            $"{movement.DateUtc:yyyy-MM-dd HH:mm} | {movement.ProductName} ({movement.ProductReference}) | {movement.Type} {movement.Quantite}");
                    }

                    if (trend.Count > 0)
                    {
                        var last = trend[^1];
                        column.Item().PaddingTop(8).Text("Dernier point de tendance").Bold().FontSize(14);
                        column.Item().Text(
                            $"{last.DateUtc:yyyy-MM-dd}: Entrées={last.TotalEntries}, Sorties={last.TotalExits}, Net={last.NetMovement}");
                    }
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("StockSense");
                    text.Span(" - ");
                    text.CurrentPageNumber();
                });
            });
        });

        var pdfData = document.GeneratePdf();
        var fileName = $"stock-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf";
        const string contentType = "application/pdf";
        return Task.FromResult((pdfData, fileName, contentType));
    }

    private static void EnsureLicense()
    {
        if (_licenseConfigured) return;

        lock (Sync)
        {
            if (_licenseConfigured) return;
            QuestPDF.Settings.License = LicenseType.Community;
            _licenseConfigured = true;
        }
    }
}

