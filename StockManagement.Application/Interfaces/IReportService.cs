using StockManagement.Application.Contracts;

namespace StockManagement.Application.Interfaces;

public interface IReportService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(int recentDays = 7);
    Task<IReadOnlyList<CategoryStockStatDto>> GetCategoryStockStatsAsync();
    Task<IReadOnlyList<StockTrendPointDto>> GetStockTrendAsync(int days = 30);
    Task<IReadOnlyList<RecentMovementDto>> GetRecentMovementsAsync(int limit = 20);
    Task<GeneratedReportDto> ExportPdfAsync(ReportRequestDto request, Guid? userId);
    Task<GeneratedReportDto> ExportExcelAsync(ReportRequestDto request, Guid? userId);
    Task<GeneratedReportDto> SendReportByEmailAsync(EmailReportRequestDto request, Guid? userId);
}

