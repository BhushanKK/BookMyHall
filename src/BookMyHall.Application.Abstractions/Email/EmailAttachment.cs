namespace BookMyHall.Application.Abstractions.Email;

public sealed class EmailAttachment
{
    public required string FilePath { get; init; }
    public required string ContentId { get; init; }
}