using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Masters;
public class District : BaseEntity
{
    public Guid DistrictId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public Guid StateId { get; set; }
    public bool IsActive { get; set; } 
    public bool IsDeleted { get; set; }
}