using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Audit;
public class ApiRequestLog : BaseEntity
{
    public Guid ApiRequestLogId { get; set; }
    public Guid UserId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public string RequestIpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public int ExecutionTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}