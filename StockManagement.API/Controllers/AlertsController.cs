using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;

namespace StockManagement.API.Controllers;

[ApiController]
[Route("api/alerts")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly IAlertService _alertService;

    public AlertsController(IStockService stockService, IAlertService alertService)
    {
        _stockService = stockService;
        _alertService = alertService;
    }

    [HttpGet("low-stock-products")]
    public async Task<ActionResult<IReadOnlyList<LowStockProductDto>>> GetLowStockProducts()
    {
        var low = await _stockService.GetLowStockProductsAsync();
        return Ok(low);
    }

    [HttpGet("low-stock-alerts")]
    [Authorize(Roles = "Admin,Responsable")]
    public async Task<ActionResult<IReadOnlyList<AlertDto>>> GetLowStockAlerts([FromQuery] int limit = 100)
    {
        var alerts = await _alertService.GetLowStockAlertsAsync(limit);
        return Ok(alerts);
    }
}

