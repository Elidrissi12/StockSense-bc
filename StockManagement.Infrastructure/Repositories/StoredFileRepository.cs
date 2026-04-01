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

    public async Task<(IReadOnlyList<StoredFile> Items, int TotalItems)> QueryAsync(string? search, string? tag, int page, int pageSize)
    {
        var query = _db.StoredFiles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim();
            query = query.Where(f => f.FileName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var normalizedTag = tag.Trim().ToLowerInvariant();
            query = query.Where(f => f.Tag == normalizedTag);
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        return (items, totalItems);
    }

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

