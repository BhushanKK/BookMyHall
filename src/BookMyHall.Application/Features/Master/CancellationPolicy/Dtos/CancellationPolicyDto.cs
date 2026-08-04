using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Master;

public class CancellationPolicyDto
{
    [JsonIgnore]
    public Guid CancellationPolicyId { get; set; }

    public string PolicyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal RefundPercentage { get; set; }

    public int CancellationBeforeHours { get; set; }

    public bool IsActive { get; set; }
}