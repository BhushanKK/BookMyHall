using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class BookingStatusHistory : BaseEntity
{
    public Guid BookingStatusHistoryId { get; set; }
    public Guid BookingId { get; set; }
    public string OldStatus { get; set; }=string.Empty;
    public string NewStatus { get; set; }=string.Empty;
    public string Remarks { get; set; }=string.Empty;
    public Guid ChangeBy { get; set; }
}