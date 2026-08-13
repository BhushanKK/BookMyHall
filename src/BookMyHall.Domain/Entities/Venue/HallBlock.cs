using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Venue;
public class HallBlock : BaseEntity
{
    public Guid HallBlockId { get; set; }
    public Guid HallId { get; set; }
    public DateOnly BlockFromDate { get; set; }
    public DateOnly BlockToDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}