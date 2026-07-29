using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class AuditLog : BaseEntity
{
    public Guid AuditLogId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string Operation { get; set; }=string.Empty;
    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; } 
}