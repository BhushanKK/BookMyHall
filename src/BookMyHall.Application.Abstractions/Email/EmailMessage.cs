namespace BookMyHall.Application.Abstractions.Email;

public sealed class EmailMessage
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
     public IReadOnlyCollection<EmailAttachment> InlineAttachments { get; init; }
        = [];
}