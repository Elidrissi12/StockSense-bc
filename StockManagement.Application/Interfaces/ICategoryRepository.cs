using StockManagement.Domain.Entities;

namespace StockManagement.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task<bool> ExistsByNameAsync(string name);
}

