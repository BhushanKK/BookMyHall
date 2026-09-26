using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Venue;
public class VendorDocument : BaseEntity
{
    public Guid VendorDocumentId { get; set; }
    public Guid VendorId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSize { get; set; }
    public string VerificationStatus { get; set; } = "Pending";
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public virtual Vendors Vendor { get; set; } = null!;
}
