using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace BookMyHall.Infrastructure.Email;

public sealed class EmailTemplateService(IOptions<EmailOptions> options)
    : IEmailTemplateService
{
    private readonly EmailOptions _options = options.Value;

    public async Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            _options.TemplateFolder,
            $"{templateName}.html");

        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        var html = await File.ReadAllTextAsync(path, cancellationToken);

        foreach (var placeholder in placeholders)
        {
            html = html.Replace(
                $"{{{{{placeholder.Key}}}}}",
                placeholder.Value);
        }

        return html;
    }
}