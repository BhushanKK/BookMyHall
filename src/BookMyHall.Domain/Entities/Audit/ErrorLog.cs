using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class ErrorLog : BaseEntity
{
    public Guid ErrorLogId { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid UserId { get; set; }
    public string RequestPath { get; set; } = string.Empty;
    public string? HttpMethod { get; set; }
    public string? ExceptionType { get; set; }
    public string? ErrorMessage { get; set; }
    public string StackTrace {get; set; } = string.Empty;
    public string? InnerException { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}