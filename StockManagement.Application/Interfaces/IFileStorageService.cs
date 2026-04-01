using StockManagement.Application.Contracts;

namespace StockManagement.Application.Interfaces;

public interface IFileStorageService
{
    Task<StoredFileMetadataDto> SaveAsync(StoredFileCreateRequest request);
    Task<PagedResult<StoredFileMetadataDto>> QueryAsync(StoredFileQuery query);
    Task<StoredFileMetadataDto?> GetMetadataAsync(Guid id);
    Task<StoredFileDownloadDto?> DownloadAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}

