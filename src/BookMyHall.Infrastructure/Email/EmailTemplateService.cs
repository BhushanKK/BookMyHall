using BookMyHall.Application.Abstractions.Email;
using Microsoft.Extensions.Options;

namespace BookMyHall.Infrastructure.Email;

public sealed class EmailTemplateService(IOptions<EmailOptions> options) 
    : IEmailTemplateService
{
    private readonly EmailOptions _options = options.Value;

    public async Task<string> RenderAsync(string templateName,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(templateName))
        {
            throw new ArgumentException(
                "Template name is required.",
                nameof(templateName));
        }

        var fileName = templateName.EndsWith(
            ".html",
            StringComparison.OrdinalIgnoreCase)
            ? templateName
            : $"{templateName}.html";

        var templateFolder = _options.TemplateFolder;

        var path = Path.Combine(AppContext.BaseDirectory, templateFolder, fileName);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException
            (
                $"Email template '{fileName}' was not found. " +
                $"Expected path: '{path}'",
                path
            );
        }

        var html = await File.ReadAllTextAsync(path, cancellationToken);

        foreach (var placeholder in placeholders)
        {
            html = html.Replace
            (
                $"{{{{{placeholder.Key}}}}}",
                placeholder.Value,
                StringComparison.Ordinal
            );
        }

        return html;
    }
}