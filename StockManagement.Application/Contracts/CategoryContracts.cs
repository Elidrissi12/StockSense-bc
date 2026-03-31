using System.ComponentModel.DataAnnotations;

namespace StockManagement.Application.Contracts;

public record CategoryDto(Guid Id, string Name);

public record CategoryCreateRequest([Required] string Name);

public record CategoryUpdateRequest([Required] string Name);

