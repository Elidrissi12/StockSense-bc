using StockManagement.Application.Contracts;

namespace StockManagement.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(CategoryCreateRequest request);
    Task<bool> UpdateAsync(Guid id, CategoryUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}

