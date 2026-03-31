using StockManagement.Domain.Enums;

namespace StockManagement.Domain.Entities;

public class Alert
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public AlertType Type { get; set; }
    public string Message { get; set; } = string.Empty;

    public DateTime DateUtc { get; set; } = DateTime.UtcNow;
}

