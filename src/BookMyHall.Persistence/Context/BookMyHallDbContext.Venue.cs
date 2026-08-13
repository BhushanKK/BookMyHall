using BookMyHall.Domain.Masters;
using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
     public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<HallImage> HallImages => Set<HallImage>();
    public DbSet<HallPricing> HallPricings => Set<HallPricing>();
    public DbSet<HallBlock> HallBlocks => Set<HallBlock>();
    public DbSet<HallCategory> HallCategories => Set<HallCategory>();
}