using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole", "identity");
        builder.HasKey(x =>  x.UserRoleId );
        builder.Property(x => x.UserRoleId).HasDefaultValueSql("gen_random_uuid()");;
    }
}