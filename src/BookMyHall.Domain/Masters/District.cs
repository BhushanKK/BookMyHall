using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class District : BaseEntity
{
    public Guid DistrictId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public Guid StateId { get; set; }
}