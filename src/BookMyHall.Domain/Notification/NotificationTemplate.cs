using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class NotificationTemplate : BaseEntity
{
    public Guid NotificationTemplateId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}