using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Persistence.Context;

public class BookMyHallDbContext : DbContext
{
    public BookMyHallDbContext(DbContextOptions<BookMyHallDbContext> options)
    : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable(
                "Role",
                "identity");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id)
                  .HasColumnName("RoleId")
                  .HasDefaultValueSql(
                     "gen_random_uuid()");

            entity.Property(x => x.RoleName)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.HasIndex(x => x.RoleName)
                  .IsUnique();

            entity.Property(x => x.CreatedDate)
                  .HasDefaultValueSql(
                     "CURRENT_TIMESTAMP");
        });
    }
}