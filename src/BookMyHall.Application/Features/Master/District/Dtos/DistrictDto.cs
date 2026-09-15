using System.Text.Json.Serialization;

using BookMyHall.Application.Common.Json;

namespace BookMyHall.Application.Features.Master;

public class DistrictDto
{
    [JsonIgnore]
    public Guid DistrictId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? StateId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}