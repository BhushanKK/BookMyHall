using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission", "identity");
        builder.HasKey(x =>  x.RolePermissionId );
        builder.Property(x => x.RolePermissionId).HasDefaultValueSql("gen_random_uuid()");;
    }
}