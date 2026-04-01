namespace StockManagement.Application.Contracts;

public record StoredFileCreateRequest(
    string FileName,
    string ContentType,
    byte[] Data,
    string Tag,
    Guid? CreatedByUserId);

public record StoredFileMetadataDto(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    string Tag,
    DateTime CreatedAtUtc,
    Guid? CreatedByUserId);

public record StoredFileDownloadDto(
    Guid Id,
    string FileName,
    string ContentType,
    byte[] Data);

public record StoredFileQuery(
    string? Search,
    string? Tag,
    int Page = 1,
    int PageSize = 20);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);

