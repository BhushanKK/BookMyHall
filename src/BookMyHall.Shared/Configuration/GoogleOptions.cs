namespace BookMyHall.Shared.Configuration;

public sealed class GoogleOptions
{
    public const string SectionName = "Google";
    public string ClientId { get; set; } = string.Empty;
}