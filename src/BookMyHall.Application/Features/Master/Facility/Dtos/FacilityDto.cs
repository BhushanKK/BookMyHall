namespace BookMyHall.Application.Features.Master;

public class FacilityDto
{
    public Guid FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityIcon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}