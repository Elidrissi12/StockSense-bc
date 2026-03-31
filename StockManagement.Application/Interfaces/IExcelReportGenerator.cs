using StockManagement.Application.Contracts;

namespace StockManagement.Application.Interfaces;

public interface IExcelReportGenerator
{
    Task<(byte[] Data, string FileName, string ContentType)> GenerateStockReportAsync(
        DashboardSummaryDto summary,
        IReadOnlyList<CategoryStockStatDto> categoryStats,
        IReadOnlyList<StockTrendPointDto> trend,
        IReadOnlyList<RecentMovementDto> recentMovements);
}

