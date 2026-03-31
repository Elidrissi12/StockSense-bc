using StockManagement.Application.Contracts;
using StockManagement.Domain.Enums;

namespace StockManagement.Application.Interfaces;

public interface IStockService
{
    Task<IReadOnlyList<StockMovementDto>> GetMovementsByProductIdAsync(Guid productId);
    Task<StockMovementDto> AddMovementAsync(Guid productId, StockMovementType type, decimal quantite, Guid userId);
    Task<IReadOnlyList<LowStockProductDto>> GetLowStockProductsAsync();
}

