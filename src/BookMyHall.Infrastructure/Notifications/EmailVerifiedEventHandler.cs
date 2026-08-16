using MediatR;
using Microsoft.AspNetCore.Hosting;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;

namespace BookMyHall.Infrastructure.Notifications;

public sealed class EmailVerifiedEventHandler(
    IEmailTemplateService emailTemplateService,
    IEmailSender emailSender,
    IWebHostEnvironment environment)
    : INotificationHandler<EmailVerifiedEvent>
{
    public async Task Handle(
        EmailVerifiedEvent notification,
        CancellationToken cancellationToken)
    {
        var logoPath = Path.Combine(environment.WebRootPath, "images", "logo.png");

        if (!File.Exists(logoPath))
            throw new FileNotFoundException("Email logo not found.", logoPath);

        var html = await emailTemplateService.RenderAsync(
            "EmailVerified",
            new Dictionary<string, string>
            {
                ["UserName"] = notification.UserName,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            },
            cancellationToken);

        var email = new EmailMessage
        {
            To = notification.Email,
            Subject = "Your email has been verified 🎉",
            HtmlBody = html,
            InlineAttachments =
            [
                new EmailAttachment
                {
                    FilePath = logoPath,
                    ContentId = "bookmyhall-logo"
                }
            ]
        };

        await emailSender.SendAsync(email, cancellationToken);
    }
}