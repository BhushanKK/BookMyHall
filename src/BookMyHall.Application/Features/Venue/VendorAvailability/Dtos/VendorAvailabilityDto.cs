using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Venue;
public class VendorAvailabilityDto
{
    [JsonIgnore]
    public Guid VendorAvailabilityId { get; set; }

    public Guid VendorId { get; set; }

    public short? DayOfWeek { get; set; }

    public DateOnly? AvailableDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsAvailable { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; }
}