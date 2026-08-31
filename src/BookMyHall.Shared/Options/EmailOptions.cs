namespace BookMyHall.Shared.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string TemplateFolder { get; set; } = "Email/Templates";
    public int VerificationExpiryMinutes { get; set; } = 30;
    public int PasswordResetExpiryMinutes { get; set; } = 30;
    public string LogoPath { get; set; } = "www/images/logo.png";
}