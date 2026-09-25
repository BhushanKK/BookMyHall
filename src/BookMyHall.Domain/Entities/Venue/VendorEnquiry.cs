using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorEnquiry : BaseEntity
{
    public Guid VendorEnquiryId { get; set; }
    public string EnquiryNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? VendorServiceId { get; set; }
    public Guid? VendorPackageId { get; set; }
    public Guid? EventCategoryId { get; set; }
    public DateOnly? EventDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? GuestCount { get; set; }
    public string? Message { get; set; }
    public decimal? ExpectedBudget { get; set; }
    public string Status { get; set; } = "New";
    public string? VendorResponse { get; set; }
    public DateTime? VendorResponseDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
    public virtual VendorService? VendorService { get; set; }
    public virtual VendorPackage? VendorPackage { get; set; }
}
