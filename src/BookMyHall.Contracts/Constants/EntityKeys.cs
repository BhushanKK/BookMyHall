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

    public const string Area = nameof(Area);
    public const string AreaId = nameof(AreaId);

    public const string CancellationPolicy = nameof(CancellationPolicy);
    public const string CancellationPolicyId = nameof(CancellationPolicyId);

    public const string City = nameof(City);
    public const string CityId = nameof(CityId);

    public const string District = nameof(District);
    public const string DistrictId = nameof(DistrictId);

    public const string EventCategory = nameof(EventCategory);
    public const string EventCategoryId = nameof(EventCategoryId);
}