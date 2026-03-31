using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.Infrastructure.Repositories;

public class StoredFileRepository : IStoredFileRepository
{
    private readonly StockDbContext _db;

    public StoredFileRepository(StockDbContext db)
    {
        _db = db;
    }

    public Task<StoredFile?> GetByIdAsync(Guid id) =>
        _db.StoredFiles.FirstOrDefaultAsync(f => f.Id == id);

    public async Task AddAsync(StoredFile file)
    {
        _db.StoredFiles.Add(file);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(StoredFile file)
    {
        _db.StoredFiles.Remove(file);
        await _db.SaveChangesAsync();
    }
}

