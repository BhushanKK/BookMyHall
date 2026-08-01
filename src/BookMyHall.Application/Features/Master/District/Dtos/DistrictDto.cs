namespace BookMyHall.Application.Features.Master;

public class DistrictDto
{
    public Guid DistrictId { get; set; }
    public Guid StateId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}