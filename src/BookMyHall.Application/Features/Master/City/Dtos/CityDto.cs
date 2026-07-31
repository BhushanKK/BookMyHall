namespace BookMyHall.Application.Features.Master;

public class CityDto
{
    public Guid CityId { get; set; }

    public Guid DistrictId { get; set; }

    public string CityName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}