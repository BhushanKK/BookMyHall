using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class Amenity:BaseEntity
{
    public Guid AmenityId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public string AmenityIcon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}