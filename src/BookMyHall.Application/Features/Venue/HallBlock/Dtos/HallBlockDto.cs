using System.Text.Json.Serialization;

namespace BookMyHall.Application.Features.Venue;

public class HallBlockDto
{
    [JsonIgnore]
    public Guid HallBlockId { get; set; }

    public Guid HallId { get; set; }

    public DateOnly BlockFromDate { get; set; }

    public DateOnly BlockToDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; }
}