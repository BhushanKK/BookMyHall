using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Masters;   
public class HallCategory: BaseEntity
{
    public Guid HallCategoryId { get; set; }
    public string HallCategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } 
    public bool IsDeleted { get; set; }
}