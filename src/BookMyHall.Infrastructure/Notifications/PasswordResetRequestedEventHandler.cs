using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Infrastructure.Options;
using MimeKit.Utils;

namespace BookMyHall.Infrastructure.Notifications;

public sealed class PasswordResetRequestedEventHandler(
    IEmailTemplateService emailTemplateService,
    IEmailSender emailSender,
    IOptions<FrontendOptions> frontendOptions,
    IWebHostEnvironment environment)
    : INotificationHandler<PasswordResetRequestedEvent>
{
    private const int PasswordResetTokenExpiryInMinutes = 30;

    public async Task Handle(PasswordResetRequestedEvent notification, CancellationToken cancellationToken)
    {
        var resetLink =
            $"{frontendOptions.Value.BaseUrl}/reset-password" +
            $"?email={Uri.EscapeDataString(notification.Email)}" +
            $"&token={Uri.EscapeDataString(notification.ResetToken)}";

        var logoPath = Path.Combine(environment.WebRootPath,"images", "logo.png");

        if (!File.Exists(logoPath))
            throw new FileNotFoundException("Email logo not found", logoPath);
        var logoCid = MimeUtils.GenerateMessageId();

        var html = await emailTemplateService.RenderAsync(
            templateName: "PasswordReset",
            placeholders: new Dictionary<string, string>
            {
                ["UserName"] = notification.UserName,
                ["ResetLink"] = resetLink,
                ["ExpiryMinutes"] = PasswordResetTokenExpiryInMinutes.ToString(),
                ["CurrentYear"] =DateTime.UtcNow.Year.ToString(),
                ["LogoCid"] = logoCid
            },
            cancellationToken);

        var emailMessage = new EmailMessage
        {
            To = notification.Email,
            Subject = "Reset your BookMyHall password",
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

        await emailSender.SendAsync(emailMessage, cancellationToken);
    }
}