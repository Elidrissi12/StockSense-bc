using StockManagement.Domain.Enums;

namespace StockManagement.Application.Contracts;

public record AlertDto(
    Guid Id,
    Guid ProductId,
    AlertType Type,
    string Message,
    DateTime DateUtc);

