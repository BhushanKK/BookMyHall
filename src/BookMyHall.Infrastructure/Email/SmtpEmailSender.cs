using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Shared.Options;

namespace BookMyHall.Infrastructure.Email;

public sealed class SmtpEmailSender(
    IOptions<EmailOptions> emailOptions)
    : IEmailSender
{
    private readonly EmailOptions _options = emailOptions.Value;

    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        // ============================================================
        // Sender
        // ============================================================

        email.From.Add(
            new MailboxAddress(
                _options.FromName,
                _options.FromEmail));

        // ============================================================
        // Recipient
        // ============================================================

        email.To.Add(
            MailboxAddress.Parse(message.To));

        // ============================================================
        // Subject
        // ============================================================

        email.Subject = message.Subject;

        // ============================================================
        // HTML body
        // ============================================================

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody
        };

        // ============================================================
        // Inline attachments
        // ============================================================

        foreach (var attachment in message.InlineAttachments)
        {
            if (!File.Exists(attachment.FilePath))
            {
                throw new FileNotFoundException(
                    $"Inline email attachment was not found: {attachment.FilePath}",
                    attachment.FilePath);
            }

            var image = bodyBuilder.LinkedResources.Add(
                attachment.FilePath);

            image.ContentId = attachment.ContentId;

            image.ContentDisposition =
                new ContentDisposition(
                    ContentDisposition.Inline);

            image.ContentLocation =
                new Uri($"cid:{attachment.ContentId}");
        }

        email.Body = bodyBuilder.ToMessageBody();

        // ============================================================
        // SMTP
        // ============================================================

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtpClient.AuthenticateAsync(
            _options.UserName,
            _options.Password,
            cancellationToken);

        await smtpClient.SendAsync(
            email,
            cancellationToken);

        await smtpClient.DisconnectAsync(
            true,
            cancellationToken);
    }
}