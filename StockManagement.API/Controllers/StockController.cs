using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Enums;

namespace StockManagement.API.Controllers;

[ApiController]
[Route("api/stock")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("products/{productId:guid}/movements")]
    public async Task<ActionResult<IReadOnlyList<StockMovementDto>>> GetMovementsByProduct([FromRoute] Guid productId)
    {
        var movements = await _stockService.GetMovementsByProductIdAsync(productId);
        return Ok(movements);
    }

    [HttpPost("movements")]
    public async Task<ActionResult<StockMovementDto>> AddMovement([FromBody] StockMovementCreateRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var created = await _stockService.AddMovementAsync(request.ProductId, request.Type, request.Quantite, userId);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException("Utilisateur non authentifié.");
        return Guid.Parse(raw);
    }
}

