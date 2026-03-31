using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly StockDbContext _db;

    public CategoryRepository(StockDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync()
    {
        var result = await _db.Categories.AsNoTracking().ToArrayAsync();
        return result;
    }

    public Task<Category?> GetByIdAsync(Guid id) =>
        _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public Task<bool> ExistsAsync(Guid id) => _db.Categories.AnyAsync(c => c.Id == id);

    public async Task AddAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }

    public Task<bool> ExistsByNameAsync(string name) =>
        _db.Categories.AnyAsync(c => c.Name == name);
}

