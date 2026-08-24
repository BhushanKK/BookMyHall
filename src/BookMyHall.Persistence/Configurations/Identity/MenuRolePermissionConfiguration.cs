using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class MenuRolePermissionConfiguration : IEntityTypeConfiguration<MenuRolePermission>
{
    public void Configure(EntityTypeBuilder<MenuRolePermission> builder)
    {
        builder.ToTable("MenuRolePermission", "identity");
        builder.HasKey(x => x.MenuRolePermissionId);
    }
}