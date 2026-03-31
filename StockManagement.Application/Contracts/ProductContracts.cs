using System.ComponentModel.DataAnnotations;

namespace StockManagement.Application.Contracts;

public record ProductDto(
    Guid Id,
    string Nom,
    string Reference,
    Guid CategorieId,
    decimal Quantite,
    string Unite,
    string Localisation,
    decimal SeuilMin);

public record ProductCreateRequest(
    [Required] string Nom,
    [Required] string Reference,
    [Required] Guid CategorieId,
    decimal Quantite,
    [Required] string Unite,
    [Required] string Localisation,
    decimal SeuilMin);

public record ProductUpdateRequest(
    [Required] string Nom,
    [Required] string Reference,
    [Required] Guid CategorieId,
    decimal Quantite,
    [Required] string Unite,
    [Required] string Localisation,
    decimal SeuilMin);

