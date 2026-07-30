using BookMyHall.Domain.Common; 

namespace BookMyHall.Domain.Masters;
public class Area : BaseEntity
{
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public Guid CityId { get; set; }
    public bool IsActive { get; set; } 
}
