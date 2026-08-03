namespace BookMyHall.Application.Abstractions.Email;

public interface IEmailTemplateService
{
    Task<string> RenderAsync(string templateName,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default);
}