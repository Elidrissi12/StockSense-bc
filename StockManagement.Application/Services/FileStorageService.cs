using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;

namespace StockManagement.Application.Services;

public class FileStorageService : IFileStorageService
{
    private const int MaxUploadSizeBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-excel",
        "image/png",
        "image/jpeg",
        "text/csv"
    };

    private readonly IStoredFileRepository _storedFileRepository;

    public FileStorageService(IStoredFileRepository storedFileRepository)
    {
        _storedFileRepository = storedFileRepository;
    }

    public async Task<StoredFileMetadataDto> SaveAsync(StoredFileCreateRequest request)
    {
        if (request.Data.Length == 0)
            throw new InvalidOperationException("Le fichier est vide.");

        if (request.Data.Length > MaxUploadSizeBytes)
            throw new InvalidOperationException("Le fichier dépasse la taille maximum autorisée (10 MB).");

        if (!AllowedContentTypes.Contains(request.ContentType))
            throw new InvalidOperationException("Type de fichier non autorisé.");

        var storedFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            FileName = SanitizeFileName(request.FileName),
            ContentType = request.ContentType,
            SizeBytes = request.Data.LongLength,
            Data = request.Data,
            Tag = string.IsNullOrWhiteSpace(request.Tag) ? "general" : request.Tag.Trim().ToLowerInvariant(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = request.CreatedByUserId
        };

        await _storedFileRepository.AddAsync(storedFile);
        return ToMetadata(storedFile);
    }

    public async Task<StoredFileMetadataDto?> GetMetadataAsync(Guid id)
    {
        var file = await _storedFileRepository.GetByIdAsync(id);
        return file is null ? null : ToMetadata(file);
    }

    public async Task<StoredFileDownloadDto?> DownloadAsync(Guid id)
    {
        var file = await _storedFileRepository.GetByIdAsync(id);
        if (file is null) return null;

        return new StoredFileDownloadDto(file.Id, file.FileName, file.ContentType, file.Data);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var file = await _storedFileRepository.GetByIdAsync(id);
        if (file is null) return false;

        await _storedFileRepository.DeleteAsync(file);
        return true;
    }

    private static StoredFileMetadataDto ToMetadata(StoredFile f) =>
        new(f.Id, f.FileName, f.ContentType, f.SizeBytes, f.Tag, f.CreatedAtUtc, f.CreatedByUserId);

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "file.bin" : cleaned;
    }
}

