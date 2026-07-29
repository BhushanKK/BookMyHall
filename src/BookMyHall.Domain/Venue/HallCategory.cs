using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;   
public class HallCategory: BaseEntity
{
    public Guid HallCategoryId { get; set; }
    public string HallCategoryName { get; set; } = string.Empty;
}