using StockManagement.Domain.Entities;

namespace StockManagement.Application.Interfaces;

public interface IStockMovementRepository
{
    Task AddAsync(StockMovement movement);
    Task<IReadOnlyList<StockMovement>> GetByProductIdAsync(Guid productId);
    Task<IReadOnlyList<StockMovement>> GetRecentAsync(int limit);
    Task<IReadOnlyList<StockMovement>> GetInPeriodAsync(DateTime fromUtc, DateTime toUtc);
}

