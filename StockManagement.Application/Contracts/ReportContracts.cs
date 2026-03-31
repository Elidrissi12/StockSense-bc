using StockManagement.Domain.Enums;

namespace StockManagement.Application.Contracts;

public record DashboardSummaryDto(
    int TotalProducts,
    int CriticalProducts,
    decimal TotalStockQuantity,
    int RecentMovementsCount);

public record CategoryStockStatDto(
    Guid CategoryId,
    string CategoryName,
    int ProductCount,
    decimal TotalQuantity);

public record StockTrendPointDto(
    DateTime DateUtc,
    decimal TotalEntries,
    decimal TotalExits,
    decimal NetMovement);

public record RecentMovementDto(
    Guid MovementId,
    Guid ProductId,
    string ProductName,
    string ProductReference,
    StockMovementType Type,
    decimal Quantite,
    DateTime DateUtc,
    Guid UserId);

public record ReportRequestDto(
    DateTime? FromUtc,
    DateTime? ToUtc,
    int Limit);

public record GeneratedReportDto(
    Guid FileId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc,
    string Tag);

public record EmailReportRequestDto(
    string ToEmail,
    string Subject,
    string Body,
    string Format, // pdf | excel
    DateTime? FromUtc,
    DateTime? ToUtc);

