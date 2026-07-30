using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Review;
public class HallFavourite : BaseEntity
{
    public Guid HallFavouriteId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid HallId { get; set; }
}