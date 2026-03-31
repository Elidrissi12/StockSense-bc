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

