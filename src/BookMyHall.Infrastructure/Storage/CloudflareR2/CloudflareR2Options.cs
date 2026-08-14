namespace BookMyHall.Infrastructure.Storage.CloudflareR2;

public sealed class CloudflareR2Options
{
    public const string SectionName = "CloudflareR2";
    public string Endpoint { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "auto";
}