using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class Service: BaseEntity
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceIcon { get; set; } = string.Empty;
}