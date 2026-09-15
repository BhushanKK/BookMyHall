using System.Text.Json.Serialization;

using BookMyHall.Application.Common.Json;

namespace BookMyHall.Application.Features.Master;

public class CityDto
{
    [JsonIgnore]
    public Guid CityId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? DistrictId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}