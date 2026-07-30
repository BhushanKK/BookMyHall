using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Payments;
public class Invoice : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid BookingId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public DateTimeOffset InvoiceDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string InvoiceStatus { get; set; } = string.Empty;
    public string PdfUrl { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}