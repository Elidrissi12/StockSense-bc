using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly StockDbContext _db;

    public AlertRepository(StockDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Alert alert)
    {
        _db.Alerts.Add(alert);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Alert>> GetLowStockAlertsAsync(int limit)
    {
        var result = await _db.Alerts
            .AsNoTracking()
            .Where(a => a.Type == Domain.Enums.AlertType.StockFaible)
            .OrderByDescending(a => a.DateUtc)
            .Take(limit)
            .ToArrayAsync();
        return result;
    }
}

