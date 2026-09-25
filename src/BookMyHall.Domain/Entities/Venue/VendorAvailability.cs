using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorAvailability : BaseEntity
{
    public Guid VendorAvailabilityId { get; set; }
    public Guid VendorId { get; set; }
    public short? DayOfWeek { get; set; }
    public DateOnly? AvailableDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Reason { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
}
