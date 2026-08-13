using BookMyHall.Domain.Masters;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Amenity> Amenitys => Set<Amenity>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<FoodType> FoodTypes => Set<FoodType>();
    public DbSet<PaymentMode> PaymentModes => Set<PaymentMode>();
    public DbSet<CancellationPolicy> CancellationPolicies => Set<CancellationPolicy>();
    public DbSet<HallCategory> HallCategories => Set<HallCategory>();
}