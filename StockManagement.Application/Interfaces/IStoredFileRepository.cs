using StockManagement.Domain.Entities;

namespace StockManagement.Application.Interfaces;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(Guid id);
    Task AddAsync(StoredFile file);
    Task DeleteAsync(StoredFile file);
}

