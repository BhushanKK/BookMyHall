using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Review;
public class HallReviewReply : BaseEntity
{
    public Guid HallReviewReplyId { get; set; }
    public Guid HallReviewId { get; set; }
    public string Reply { get; set; } = string.Empty;
    public Guid RepliedBy { get; set; }
}