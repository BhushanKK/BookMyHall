using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;

public sealed class UserPreference : BaseEntity
{
    public Guid UserPreferenceId { get; private set; }
    public Guid UserId { get; private set; }
    // Regional Settings
    public string CurrencyCode { get; private set; } = "INR";
    public string TimeZone { get; private set; } = "Asia/Kolkata";
    // Date & Time
    public string DateFormat { get; private set; } = "dd-MM-yyyy";
    public string TimeFormat { get; private set; } = "24";
    // Language
    public string LanguageCode { get; private set; } = "en-IN";
    // Notification Preferences
    public bool EmailNotification { get; private set; } = true;
    public bool SmsNotification { get; private set; } = true;
    public bool PushNotification { get; private set; } = true;
    // Theme / UI
    public string Theme { get; private set; } = "Light";
    public bool IsActive {get;set;}
    // Navigation
    public User User { get; private set; } = default!;
    private UserPreference()
    {
    }

    public static UserPreference Create(Guid userId)
    {
        return new UserPreference
        {
            UserPreferenceId = Guid.NewGuid(),
            UserId = userId,
            CurrencyCode = "INR",
            TimeZone = "Asia/Kolkata",
            DateFormat = "dd-MM-yyyy",
            TimeFormat = "24",
            LanguageCode = "en-IN",
            EmailNotification = true,
            SmsNotification = true,
            PushNotification = true,
            Theme = "Light",
        };
    }

    public void Update(
        string currencyCode,
        string timeZone,
        string dateFormat,
        string timeFormat,
        string languageCode,
        bool emailNotification,
        bool smsNotification,
        bool pushNotification,
        string theme)
    {
        CurrencyCode = currencyCode;
        TimeZone = timeZone;
        DateFormat = dateFormat;
        TimeFormat = timeFormat;
        LanguageCode = languageCode;
        EmailNotification = emailNotification;
        SmsNotification = smsNotification;
        PushNotification = pushNotification;
        Theme = theme;
    }

    public void UpdateNotificationPreferences(
        bool emailNotification,
        bool smsNotification,
        bool pushNotification)
    {
        EmailNotification = emailNotification;
        SmsNotification = smsNotification;
        PushNotification = pushNotification;
    }

    public void UpdateRegionalSettings(string currencyCode,string timeZone)
    {
        CurrencyCode = currencyCode;
        TimeZone = timeZone;
    }

    public void UpdateLanguage(
        string languageCode)
    {
        LanguageCode = languageCode;
    }

    public void UpdateDateTimeFormat(
        string dateFormat,
        string timeFormat)
    {
        DateFormat = dateFormat;
        TimeFormat = timeFormat;
    }

    public void UpdateTheme(string theme)
    {
        Theme = theme;
    }
}