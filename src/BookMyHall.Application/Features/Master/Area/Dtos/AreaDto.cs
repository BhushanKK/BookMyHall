using System.Text.Json.Serialization;

using BookMyHall.Application.Common.Json;

namespace BookMyHall.Application.Features.Master;

public class AreaDto
{
    [JsonIgnore]
    public Guid AreaId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? CityId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}