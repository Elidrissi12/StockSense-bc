using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;

namespace StockManagement.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;

    public AlertService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<IReadOnlyList<AlertDto>> GetLowStockAlertsAsync(int limit = 100)
    {
        var alerts = await _alertRepository.GetLowStockAlertsAsync(limit);
        return alerts.Select(ToDto).ToArray();
    }

    private static AlertDto ToDto(Alert a) =>
        new(a.Id, a.ProductId, a.Type, a.Message, a.DateUtc);
}

