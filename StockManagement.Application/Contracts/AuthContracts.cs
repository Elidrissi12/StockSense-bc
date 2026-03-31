using StockManagement.Domain.Enums;

namespace StockManagement.Application.Contracts;

public record RegisterRequest(string Nom, string Email, UserRole Role, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(Guid UserId, string Token, DateTime ExpiresAtUtc, UserRole Role);