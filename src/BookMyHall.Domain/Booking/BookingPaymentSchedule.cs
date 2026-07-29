using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class BookingPaymentSchedule : BaseEntity
{
    public Guid BookingPaymentScheduleId { get; set; }
    public Guid BookingId { get; set; }
    public int InstallmentNo { get; set; }
    public DateTime DueDate { get; set; }
    public decimal DueAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public bool PaymentStatus { get; set; }
    public string Remarks { get; set; } = string.Empty;
}