using System.Text.Json.Serialization;

using BookMyHall.Application.Common.Json;

namespace BookMyHall.Application.Features.Venue;

public class HallPricingDto
{
    public Guid HallPricingId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? HallId { get; init; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? EventCategoryId { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public int? MinimumGuests { get; init; }
    public int? MaximumGuests { get; init; }
    public decimal? WeekdayPrice { get; init; }
    public decimal? WeekendPrice { get; init; }
    public decimal? AdvanceAmount { get; init; }
    public decimal? SecurityDeposit { get; init; }
    public decimal? ExtraGuestCharge { get; init; }
    public bool IsActive { get; init; }
}