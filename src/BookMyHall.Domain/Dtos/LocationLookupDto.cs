namespace BookMyHall.Domain.Dtos;
public sealed class LocationLookupDto
{
    // =========================================================
    // Country
    // =========================================================

    public sealed class CountryLookupDto
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string? PhoneCode { get; set; }
        public string? CurrencyCode { get; set; }
    }


    // =========================================================
    // State
    // =========================================================

    public sealed class StateLookupDto
    {
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
    }


    // =========================================================
    // District
    // =========================================================

    public sealed class DistrictLookupDto
    {
        public Guid DistrictId { get; set; }
        public Guid StateId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
    }


    // =========================================================
    // City
    // =========================================================

    public sealed class CityLookupDto
    {
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public string CityName { get; set; } = string.Empty;
    }


    // =========================================================
    // Area
    // =========================================================

    public sealed class AreaLookupDto
    {
        public Guid AreaId { get; set; }
        public Guid CityId { get; set; }
        public string AreaName { get; set; } = string.Empty;
    }
}