using MediatR;
using Microsoft.AspNetCore.Hosting;
using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;

namespace BookMyHall.Infrastructure.Notifications;

public sealed class UserRegisteredEventHandler(
    IEmailTemplateService emailTemplateService,
    IEmailSender emailSender,
    IWebHostEnvironment environment)
    : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification,
        CancellationToken cancellationToken)
    {
        var logoPath = Path.Combine(environment.WebRootPath,"images","logo.png");

        var html = await emailTemplateService.RenderAsync(
            "Welcome",
            new Dictionary<string, string>
            {
                ["UserName"] = notification.UserName,
                ["CurrentYear"] = DateTime.UtcNow.Year.ToString()
            },
            cancellationToken);

        var email = new EmailMessage
        {
            To = notification.Email,
            Subject = "Welcome to BookMyHall 🎉",
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

        await emailSender.SendAsync(email,cancellationToken);
    }
}