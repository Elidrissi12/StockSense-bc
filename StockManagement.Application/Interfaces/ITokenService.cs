using StockManagement.Domain.Enums;

namespace StockManagement.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(Guid userId, string email, UserRole role);
    Task<DateTime> GetTokenExpiryUtcAsync();
}

