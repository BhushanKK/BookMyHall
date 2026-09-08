namespace BookMyHall.Domain.Identity;
public class UserDetailsView
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsMobileVerified { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? CurrencyCode { get; set; }
    public string? TimeZone { get; set; }
    public string DateFormat { get; private set; } = "dd-MM-yyyy";
    public string LanguageCode { get; private set; } = "en-IN";
    public string Theme { get; private set; } = "Light";
    public string? RoleName { get; set; }
}