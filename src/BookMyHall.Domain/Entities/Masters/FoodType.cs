using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class FoodType : BaseEntity
{
    public Guid FoodTypeId { get; set; }
    public string FoodTypeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}