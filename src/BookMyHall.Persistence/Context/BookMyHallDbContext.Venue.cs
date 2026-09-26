using BookMyHall.Domain.Venue;

using Microsoft.EntityFrameworkCore;
namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<HallImage> HallImages => Set<HallImage>();
    public DbSet<HallPricing> HallPricings => Set<HallPricing>();
    public DbSet<HallBlock> HallBlocks => Set<HallBlock>();
    public DbSet<HallListView> HallListViews => Set<HallListView>();
    public DbSet<NearbyHallView> NearbyHallViews => Set<NearbyHallView>();
    public DbSet<Vendors> Vendors => Set<Vendors>();
    public DbSet<VendorCategory> VendorCategories => Set<VendorCategory>();
    public DbSet<VendorAvailability> VendorAvailabilities => Set<VendorAvailability>();
    public DbSet<VendorBooking> VendorBookings => Set<VendorBooking>();
    public DbSet<VendorDocument> VendorDocuments => Set<VendorDocument>();
    public DbSet<VendorEnquiry> VendorEnquiries => Set<VendorEnquiry>();
    public DbSet<VendorImage> VendorImages => Set<VendorImage>();
    public DbSet<VendorPackage> VendorPackages => Set<VendorPackage>();
    public DbSet<VendorPackageItem> VendorPackageItems => Set<VendorPackageItem>();
    public DbSet<VendorReview> VendorReviews => Set<VendorReview>();
    public DbSet<VendorService> VendorServices => Set<VendorService>();
    public DbSet<VendorServiceArea> VendorServiceAreas => Set<VendorServiceArea>();
    public DbSet<VendorSubCategory> VendorSubCategories => Set<VendorSubCategory>();

}