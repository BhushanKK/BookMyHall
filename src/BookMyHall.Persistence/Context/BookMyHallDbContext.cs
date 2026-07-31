using BookMyHall.Application.Abstractions.Security;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;

    public BookMyHallDbContext(
        DbContextOptions<BookMyHallDbContext> options,
        ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookMyHallDbContext).Assembly);
    }
}