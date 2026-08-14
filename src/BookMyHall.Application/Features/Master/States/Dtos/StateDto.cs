using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class StateDto
{
    [JsonIgnore]
    public Guid StateId { get; set; }
    public Guid CountryId { get; init; }
    public string StateName { get; init; } = string.Empty;
    public string StateCode {get; init;}=string.Empty;
    public bool IsActive { get; init; }
}