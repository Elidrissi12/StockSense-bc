namespace StockManagement.Application.Interfaces;

public interface IEmailService
{
    Task SendWithAttachmentAsync(
        string toEmail,
        string subject,
        string body,
        string attachmentFileName,
        string attachmentContentType,
        byte[] attachmentData);
}

