using StockManagement.Domain.Entities;

namespace StockManagement.Application.Interfaces;

public interface IAlertRepository
{
    Task AddAsync(Alert alert);
    Task<IReadOnlyList<Alert>> GetLowStockAlertsAsync(int limit);
}

