namespace BookMyHall.Shared.Constants;

public static class EntityKeys
{
  // Identity
    public const string Role = nameof(Role);
    public const string RoleId = nameof(RoleId);

    public const string User = nameof(User);
    public const string UserId = nameof(UserId);

    public const string Permission = nameof(Permission);
    public const string UserRole = nameof(UserRole);

    public const string MobileNumber = "MobileNumber";
    public const string Password = "Password";

    public const string FirstName = "FirstName";
    public const string EmailAddress = "EmailAddress";
    public const string Token = "Token";

    public const string Device = "Device";
    public const string DeviceIdentifier = "Device Identifier";
    public const string DeviceType = "Device Type";
    public const string DeviceName = "Device Name";
    public const string PushNotificationToken = "Push Notification Token";
    public const string OperatingSystem = "Operating System";
    public const string Browser = "Browser";
    public const string AppVersion = "App Version";
    public const string LastIpAddress = "Last IP Address";
    public const string UserPreference = "User Preference";
    public const string CurrencyCode = "Currency Code";
    public const string TimeZone = "Time Zone";
    public const string DateFormat = "Date Format";
    public const string TimeFormat = "Time Format";
    public const string LanguageCode = "Language Code";
    public const string Theme = "Theme";


    // Hall

    public const string Hall = nameof(Hall);
    public const string HallId = nameof(HallId);

    public const string Booking = nameof(Booking);
    public const string BookingId = nameof(BookingId);

    public const string Customer = nameof(Customer);
    public const string CustomerId = nameof(CustomerId);

    //Master
    public const string State = nameof(State);
    public const string StateId = nameof(StateId);

    public const string Amenity = nameof(Amenity);
    public const string AmenityId = nameof(AmenityId);
    public const string AmenityIcon = nameof(AmenityIcon);


    public const string Area = nameof(Area);
    public const string AreaId = nameof(AreaId);
    public const string Pincode = nameof(Pincode);
    public const string AreaName = nameof(AreaName);

    public const string CancellationPolicy = nameof(CancellationPolicy);
    public const string CancellationPolicyId = nameof(CancellationPolicyId);
    public const string PolicyName = nameof(PolicyName);
    public const string Description = nameof(Description);
    public const string CancellationBeforeHours = nameof(CancellationBeforeHours);

    public const string City = nameof(City);
    public const string CityId = nameof(CityId);
    public const string CityName = nameof(CityName);

    public const string District = nameof(District);
    public const string DistrictId = nameof(DistrictId);
    public const string DistrictName = nameof(DistrictName);

    public const string EventCategory = nameof(EventCategory);
    public const string EventCategoryId = nameof(EventCategoryId);
    public const string EventCategoryName = nameof(EventCategoryName);

    public const string Facility = nameof(Facility);
    public const string FacilityId = nameof(FacilityId);
    public const string FacilityName = nameof(FacilityName);
    public const string FacilityIcon = nameof(FacilityIcon);

    public const string FoodType = nameof(FoodType);
    public const string FoodTypeId = nameof(FoodTypeId);
    public const string FoodTypeName = nameof(FoodTypeName);

    public const string PaymentMode = nameof(PaymentMode);
    public const string PaymentModeId = nameof(PaymentModeId);
    public const string PaymentModeName = nameof(PaymentModeName);

    public const string Service = nameof(Service);
    public const string ServiceId = nameof(ServiceId);
    public const string ServiceName = nameof(ServiceName);
    public const string ServiceIcon = nameof(ServiceIcon);

    public const string CurrentPassword = nameof(CurrentPassword);
    public const string NewPassword = nameof(NewPassword);
    public const string ConfirmPassword = nameof(ConfirmPassword);

    //Hall
    public const string HallOwnerId = nameof(HallOwnerId);
    public const string HallCategoryId = nameof(HallCategoryId);
    public const string HallName = nameof(HallName);
    public const string AddressLine1 = nameof(AddressLine1);
    public const string ContactPersonName = nameof(ContactPersonName);
    public const string HallPricing=nameof(HallPricing);
    public const string PackageName = nameof(PackageName);
    public const string HallPricingId = nameof(HallPricingId);
}