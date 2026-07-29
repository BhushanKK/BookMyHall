using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Booking;
public class HallReviewImage : BaseEntity
{
    public Guid HallReviewImageId { get; set; }
    public Guid HallReviewId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } 
}