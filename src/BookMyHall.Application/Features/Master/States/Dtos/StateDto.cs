using System.Text.Json.Serialization;
using BookMyHall.Application.Common.Json;
namespace BookMyHall.Application.Features.Master;

public class StateDto
{
    [JsonIgnore]
    public Guid StateId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? CountryId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public string StateCode {get; set;}=string.Empty;
    public bool IsActive { get; set; }
}