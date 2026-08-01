namespace BookMyHall.Application.Features.Master;
public class FoodTypeDto
{
    public Guid FoodTypeId { get; set; }
    public string FoodTypeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}