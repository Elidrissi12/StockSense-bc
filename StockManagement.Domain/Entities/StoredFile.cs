namespace StockManagement.Domain.Entities;

public class StoredFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string Tag { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}

