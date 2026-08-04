using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class PaymentModeDto
{
    [JsonIgnore]
    public Guid PaymentModeId { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}