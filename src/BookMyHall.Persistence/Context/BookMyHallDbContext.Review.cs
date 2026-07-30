using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Review;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<HallReview> HallReviews => Set<HallReview>();
    public DbSet<HallReviewImage> HallReviewImages => Set<HallReviewImage>();
    public DbSet<HallReviewReply> HallReviewReplies => Set<HallReviewReply>();
    public DbSet<HallFavourite> HallFavourites => Set<HallFavourite>();
}