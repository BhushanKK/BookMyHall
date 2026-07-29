using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
}