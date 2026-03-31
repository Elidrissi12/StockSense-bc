using StockManagement.Application.Contracts;

namespace StockManagement.Application.Interfaces;

public interface IAlertService
{
    Task<IReadOnlyList<AlertDto>> GetLowStockAlertsAsync(int limit = 100);
}

