using MediatR;
using Microsoft.AspNetCore.Hosting;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;

namespace BookMyHall.Infrastructure.Notifications;

public sealed class PasswordChangedEventHandler(
    IEmailTemplateService emailTemplateService,
    IEmailSender emailSender,
    IWebHostEnvironment environment)
    : INotificationHandler<PasswordChangedEvent> 
{
    public async Task Handle(PasswordChangedEvent notification,CancellationToken cancellationToken)
    {
        var logoPath = Path.Combine(environment.WebRootPath,"images","logo.png");

        if (!File.Exists(logoPath))
        {
            throw new FileNotFoundException(
                "Email logo not found.",
                logoPath);
        }

        var html = await emailTemplateService.RenderAsync(
            templateName: "PasswordChanged",
            placeholders: new Dictionary<string, string>
            {
                ["UserName"] = notification.UserName,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            },
            cancellationToken);

        var emailMessage = new EmailMessage
        {
            To = notification.Email,
            Subject = "Your BookMyHall password has been changed",
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

        await emailSender.SendAsync(emailMessage,cancellationToken);
    }
}