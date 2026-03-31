using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.Infrastructure.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly StockDbContext _db;

    public StockMovementRepository(StockDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(StockMovement movement)
    {
        _db.StockMovements.Add(movement);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<StockMovement>> GetByProductIdAsync(Guid productId)
    {
        var result = await _db.StockMovements
            .AsNoTracking()
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.DateUtc)
            .ToArrayAsync();
        return result;
    }

    public async Task<IReadOnlyList<StockMovement>> GetRecentAsync(int limit)
    {
        var result = await _db.StockMovements
            .AsNoTracking()
            .OrderByDescending(m => m.DateUtc)
            .Take(limit)
            .ToArrayAsync();
        return result;
    }

    public async Task<IReadOnlyList<StockMovement>> GetInPeriodAsync(DateTime fromUtc, DateTime toUtc)
    {
        var result = await _db.StockMovements
            .AsNoTracking()
            .Where(m => m.DateUtc >= fromUtc && m.DateUtc <= toUtc)
            .OrderBy(m => m.DateUtc)
            .ToArrayAsync();
        return result;
    }
}

