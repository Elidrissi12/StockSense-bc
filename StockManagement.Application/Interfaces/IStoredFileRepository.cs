using StockManagement.Domain.Entities;

namespace StockManagement.Application.Interfaces;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(Guid id);
    Task<(IReadOnlyList<StoredFile> Items, int TotalItems)> QueryAsync(string? search, string? tag, int page, int pageSize);
    Task AddAsync(StoredFile file);
    Task DeleteAsync(StoredFile file);
}

