using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class BookingGuest : BaseEntity
{
    public Guid BookingGuestId { get; set; }
    public Guid BookingId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
}