using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Infrastructure.Options;

namespace BookMyHall.Infrastructure.Notifications;

public sealed class EmailVerificationRequestedEventHandler(
    IEmailTemplateService emailTemplateService,
    IEmailSender emailSender,
    IOptions<FrontendOptions> frontendOptions,
    IWebHostEnvironment environment)
    : INotificationHandler<EmailVerificationRequestedEvent>
{
    private const int EmailVerificationExpiryInMinutes = 30;

    public async Task Handle(
        EmailVerificationRequestedEvent notification,
        CancellationToken cancellationToken)
    {
        var baseUrl = frontendOptions.Value.BaseUrl;

        var verificationLink =
            $"{baseUrl}/verify-email" +
            $"?email={Uri.EscapeDataString(notification.Email)}" +
            $"&token={Uri.EscapeDataString(notification.VerificationToken)}";

        var logoPath = Path.Combine(environment.WebRootPath, "images", "logo.png");

        if (!File.Exists(logoPath))
            throw new FileNotFoundException("Email logo not found.", logoPath);

        var html = await emailTemplateService.RenderAsync(
            "VerifyEmail",
            new Dictionary<string, string>
            {
                ["UserName"] = notification.UserName,
                ["VerificationLink"] = verificationLink,
                ["ExpiryMinutes"] = EmailVerificationExpiryInMinutes.ToString(),
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            },
            cancellationToken);

        var email = new EmailMessage
        {
            To = notification.Email,
            Subject = "Verify your BookMyHall email address",
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