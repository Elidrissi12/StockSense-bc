using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _db;

    public ProductRepository(StockDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        var result = await _db.Products
            .AsNoTracking()
            .Include(p => p.Categorie)
            .ToArrayAsync();
        return result;
    }

    public Task<Product?> GetByIdAsync(Guid id) =>
        _db.Products
            .Include(p => p.Categorie)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }
}

