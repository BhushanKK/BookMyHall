using BookMyHall.Domain.Common; 

namespace BookMyHall.Domain.Identity;
public class Area : BaseEntity
{
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public Guid CityId { get; set; }
}
