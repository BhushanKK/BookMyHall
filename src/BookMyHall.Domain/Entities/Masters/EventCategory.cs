using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class EventCategory : BaseEntity
{
    public Guid EventCategoryId { get; set; }
    public string EventCategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}