using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Infrastructure.Options;

namespace BookMyHall.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<EmailOptions> emailOptions) : IEmailSender
{
    private readonly EmailOptions options = emailOptions.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(options.FromName, options.FromEmail));

        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody
        };

        // Add inline images
        foreach (var attachment in message.InlineAttachments)
        {
            var image = bodyBuilder.LinkedResources.Add(attachment.FilePath);
            image.ContentId = attachment.ContentId;
            image.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
            image.ContentLocation = new Uri($"cid:{attachment.ContentId}");
        }
        email.Body = bodyBuilder.ToMessageBody();
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(options.Host, options.Port, SecureSocketOptions.StartTls, cancellationToken);
        await smtpClient.AuthenticateAsync(options.UserName, options.Password, cancellationToken);
        await smtpClient.SendAsync(email, cancellationToken);
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}