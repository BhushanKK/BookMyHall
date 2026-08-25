using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class StateDto
{
    [JsonIgnore]
    public Guid StateId { get; set; }
    public Guid CountryId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public string StateCode {get; set;}=string.Empty;
    public bool IsActive { get; set; }
}