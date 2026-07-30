using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Masters;   
public class City: BaseEntity
{
    public Guid CityId { get; set; }
    public Guid DistrictId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public bool IsActive { get; set; } 
}