using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class MenuPermissionConfiguration : IEntityTypeConfiguration<MenuPermission>
{
    public void Configure(EntityTypeBuilder<MenuPermission> builder)
    {
        builder.ToTable("MenuPermission", "identity");
        builder.HasKey(x => x.MenuPermissionId);
        builder.Property(x => x.MenuPermissionId).HasDefaultValueSql("gen_random_uuid()");;
    }
}