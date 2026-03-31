using System.ComponentModel.DataAnnotations;
using StockManagement.Domain.Enums;

namespace StockManagement.Application.Contracts;

public record StockMovementDto(
    Guid Id,
    Guid ProductId,
    StockMovementType Type,
    decimal Quantite,
    DateTime DateUtc,
    Guid UserId);

public record StockMovementCreateRequest(
    [Required] Guid ProductId,
    [Required] StockMovementType Type,
    decimal Quantite);

public record LowStockProductDto(
    Guid ProductId,
    string Nom,
    string Reference,
    decimal Quantite,
    decimal SeuilMin,
    string Localisation);

