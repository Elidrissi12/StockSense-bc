using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;

namespace StockManagement.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto(c.Id, c.Name)).ToArray();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category is null ? null : new CategoryDto(category.Id, category.Name);
    }

    public async Task<CategoryDto> CreateAsync(CategoryCreateRequest request)
    {
        var normalized = request.Name.Trim();
        if (await _categoryRepository.ExistsByNameAsync(normalized))
            throw new InvalidOperationException("Une catégorie avec ce nom existe déjà.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = normalized
        };

        await _categoryRepository.AddAsync(category);
        return new CategoryDto(category.Id, category.Name);
    }

    public async Task<bool> UpdateAsync(Guid id, CategoryUpdateRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) return false;

        var normalized = request.Name.Trim();
        if (!string.Equals(category.Name, normalized, StringComparison.OrdinalIgnoreCase) &&
            await _categoryRepository.ExistsByNameAsync(normalized))
            throw new InvalidOperationException("Une catégorie avec ce nom existe déjà.");

        category.Name = normalized;
        await _categoryRepository.UpdateAsync(category);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) return false;

        await _categoryRepository.DeleteAsync(category);
        return true;
    }
}

