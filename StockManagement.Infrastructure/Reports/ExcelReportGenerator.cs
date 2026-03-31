using ClosedXML.Excel;
using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;

namespace StockManagement.Infrastructure.Reports;

public class ExcelReportGenerator : IExcelReportGenerator
{
    public Task<(byte[] Data, string FileName, string ContentType)> GenerateStockReportAsync(
        DashboardSummaryDto summary,
        IReadOnlyList<CategoryStockStatDto> categoryStats,
        IReadOnlyList<StockTrendPointDto> trend,
        IReadOnlyList<RecentMovementDto> recentMovements)
    {
        using var workbook = new XLWorkbook();

        var wsSummary = workbook.AddWorksheet("Summary");
        wsSummary.Cell("A1").Value = "Metric";
        wsSummary.Cell("B1").Value = "Value";
        wsSummary.Cell("A2").Value = "TotalProducts";
        wsSummary.Cell("B2").Value = summary.TotalProducts;
        wsSummary.Cell("A3").Value = "CriticalProducts";
        wsSummary.Cell("B3").Value = summary.CriticalProducts;
        wsSummary.Cell("A4").Value = "TotalStockQuantity";
        wsSummary.Cell("B4").Value = summary.TotalStockQuantity;
        wsSummary.Cell("A5").Value = "RecentMovementsCount";
        wsSummary.Cell("B5").Value = summary.RecentMovementsCount;
        wsSummary.Columns().AdjustToContents();

        var wsCategory = workbook.AddWorksheet("CategoryStats");
        wsCategory.Cell(1, 1).Value = "Category";
        wsCategory.Cell(1, 2).Value = "ProductCount";
        wsCategory.Cell(1, 3).Value = "TotalQuantity";
        for (var i = 0; i < categoryStats.Count; i++)
        {
            var row = i + 2;
            wsCategory.Cell(row, 1).Value = categoryStats[i].CategoryName;
            wsCategory.Cell(row, 2).Value = categoryStats[i].ProductCount;
            wsCategory.Cell(row, 3).Value = categoryStats[i].TotalQuantity;
        }
        wsCategory.Columns().AdjustToContents();

        var wsTrend = workbook.AddWorksheet("Trend");
        wsTrend.Cell(1, 1).Value = "DateUtc";
        wsTrend.Cell(1, 2).Value = "Entries";
        wsTrend.Cell(1, 3).Value = "Exits";
        wsTrend.Cell(1, 4).Value = "Net";
        for (var i = 0; i < trend.Count; i++)
        {
            var row = i + 2;
            wsTrend.Cell(row, 1).Value = trend[i].DateUtc;
            wsTrend.Cell(row, 2).Value = trend[i].TotalEntries;
            wsTrend.Cell(row, 3).Value = trend[i].TotalExits;
            wsTrend.Cell(row, 4).Value = trend[i].NetMovement;
        }
        wsTrend.Columns().AdjustToContents();

        var wsRecent = workbook.AddWorksheet("RecentMovements");
        wsRecent.Cell(1, 1).Value = "DateUtc";
        wsRecent.Cell(1, 2).Value = "Product";
        wsRecent.Cell(1, 3).Value = "Reference";
        wsRecent.Cell(1, 4).Value = "Type";
        wsRecent.Cell(1, 5).Value = "Quantity";
        for (var i = 0; i < recentMovements.Count; i++)
        {
            var row = i + 2;
            wsRecent.Cell(row, 1).Value = recentMovements[i].DateUtc;
            wsRecent.Cell(row, 2).Value = recentMovements[i].ProductName;
            wsRecent.Cell(row, 3).Value = recentMovements[i].ProductReference;
            wsRecent.Cell(row, 4).Value = recentMovements[i].Type.ToString();
            wsRecent.Cell(row, 5).Value = recentMovements[i].Quantite;
        }
        wsRecent.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);

        var fileName = $"stock-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
        const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        return Task.FromResult((ms.ToArray(), fileName, contentType));
    }
}

