using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using StockManagement.Application.Interfaces;

namespace StockManagement.Infrastructure.Communication;

public class SmtpEmailService : IEmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _user;
    private readonly string _password;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public SmtpEmailService(IConfiguration configuration)
    {
        var smtp = configuration.GetSection("Smtp");

        _host = smtp["Host"] ?? throw new InvalidOperationException("Smtp:Host manquant.");
        _port = int.TryParse(smtp["Port"], out var parsedPort) ? parsedPort : 587;
        _user = smtp["User"] ?? string.Empty;
        _password = smtp["Password"] ?? string.Empty;
        _fromEmail = smtp["FromEmail"] ?? throw new InvalidOperationException("Smtp:FromEmail manquant.");
        _fromName = smtp["FromName"] ?? "StockSense";
        _enableSsl = bool.TryParse(smtp["EnableSsl"], out var parsedSsl) && parsedSsl;
    }

    public async Task SendWithAttachmentAsync(
        string toEmail,
        string subject,
        string body,
        string attachmentFileName,
        string attachmentContentType,
        byte[] attachmentData)
    {
        var message = new MimeKit.MimeMessage();
        message.From.Add(new MimeKit.MailboxAddress(_fromName, _fromEmail));
        message.To.Add(MimeKit.MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var builder = new MimeKit.BodyBuilder
        {
            TextBody = body
        };
        builder.Attachments.Add(attachmentFileName, attachmentData, MimeKit.ContentType.Parse(attachmentContentType));
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        var secureSocketOption = _enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
        await client.ConnectAsync(_host, _port, secureSocketOption);

        if (!string.IsNullOrWhiteSpace(_user))
            await client.AuthenticateAsync(_user, _password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}

