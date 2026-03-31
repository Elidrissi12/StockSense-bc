using StockManagement.Domain.Enums;

namespace StockManagement.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    // Stocke un hash (pas le mot de passe en clair).
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

