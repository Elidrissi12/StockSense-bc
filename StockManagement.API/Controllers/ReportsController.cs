using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;

namespace StockManagement.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard-summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary([FromQuery] int recentDays = 7)
    {
        var result = await _reportService.GetDashboardSummaryAsync(recentDays);
        return Ok(result);
    }

    [HttpGet("category-stock-stats")]
    public async Task<ActionResult<IReadOnlyList<CategoryStockStatDto>>> GetCategoryStockStats()
    {
        var result = await _reportService.GetCategoryStockStatsAsync();
        return Ok(result);
    }

    [HttpGet("stock-trend")]
    public async Task<ActionResult<IReadOnlyList<StockTrendPointDto>>> GetStockTrend([FromQuery] int days = 30)
    {
        var result = await _reportService.GetStockTrendAsync(days);
        return Ok(result);
    }

    [HttpGet("recent-movements")]
    public async Task<ActionResult<IReadOnlyList<RecentMovementDto>>> GetRecentMovements([FromQuery] int limit = 20)
    {
        var result = await _reportService.GetRecentMovementsAsync(limit);
        return Ok(result);
    }

    [HttpPost("export/pdf")]
    [Authorize(Roles = "Admin,Responsable")]
    public async Task<ActionResult<GeneratedReportDto>> ExportPdf([FromBody] ReportRequestDto request)
    {
        try
        {
            var generated = await _reportService.ExportPdfAsync(request, GetCurrentUserIdOrNull());
            return Ok(generated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("export/excel")]
    [Authorize(Roles = "Admin,Responsable")]
    public async Task<ActionResult<GeneratedReportDto>> ExportExcel([FromBody] ReportRequestDto request)
    {
        try
        {
            var generated = await _reportService.ExportExcelAsync(request, GetCurrentUserIdOrNull());
            return Ok(generated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("email")]
    [Authorize(Roles = "Admin,Responsable")]
    public async Task<ActionResult<GeneratedReportDto>> SendReportByEmail([FromBody] EmailReportRequestDto request)
    {
        try
        {
            var generated = await _reportService.SendReportByEmailAsync(request, GetCurrentUserIdOrNull());
            return Ok(generated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid? GetCurrentUserIdOrNull()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}

