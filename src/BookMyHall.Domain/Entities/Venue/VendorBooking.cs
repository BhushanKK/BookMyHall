using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Venue;

public class VendorBooking : BaseEntity
{
    public Guid VendorBookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? VendorServiceId { get; set; }
    public Guid? VendorPackageId { get; set; }
    public Guid? VendorEnquiryId { get; set; }
    public Guid? EventCategoryId { get; set; }
    public DateOnly EventDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int? GuestCount { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AdvanceAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string PaymentStatus { get; set; } = "Pending";
    public string? CustomerNotes { get; set; }
    public string? VendorNotes { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
    public virtual VendorService? VendorService { get; set; }
    public virtual VendorPackage? VendorPackage { get; set; }
    public virtual VendorEnquiry? VendorEnquiry { get; set; }
}
