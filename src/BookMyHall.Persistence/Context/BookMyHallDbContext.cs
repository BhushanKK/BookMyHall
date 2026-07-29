using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext : DbContext
{
    public BookMyHallDbContext(DbContextOptions<BookMyHallDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookMyHallDbContext).Assembly);
    }
}