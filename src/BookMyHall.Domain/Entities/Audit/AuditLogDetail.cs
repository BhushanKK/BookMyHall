using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class AuditLogDetail : BaseEntity
{
    public Guid AuditLogDetailId { get; set; }
    public Guid AuditLogId { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}