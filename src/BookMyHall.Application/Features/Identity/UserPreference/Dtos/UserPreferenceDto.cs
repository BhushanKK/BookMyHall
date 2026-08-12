using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Identity;

public class UserPreferenceDto
{
    [JsonIgnore]
    public Guid UserPreferenceId { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }

    // Regional Settings
    public string CurrencyCode { get; set; } = "INR";

    public string TimeZone { get; set; } = "Asia/Kolkata";

    // Date & Time
    public string DateFormat { get; set; } = "dd-MM-yyyy";

    public string TimeFormat { get; set; } = "24";

    // Language
    public string LanguageCode { get; set; } = "en-IN";

    // Notification Preferences
    public bool EmailNotification { get; set; } = true;

    public bool SmsNotification { get; set; } = true;

    public bool PushNotification { get; set; } = true;

    // Theme / UI
    public string Theme { get; set; } = "Light";
}