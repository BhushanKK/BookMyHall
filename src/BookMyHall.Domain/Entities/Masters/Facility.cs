using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Masters;
public class Facility : BaseEntity
{
    public Guid FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityIcon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}