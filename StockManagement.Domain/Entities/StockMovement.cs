using StockManagement.Domain.Enums;

namespace StockManagement.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public StockMovementType Type { get; set; }
    public decimal Quantite { get; set; }

    public DateTime DateUtc { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User? User { get; set; }
}

