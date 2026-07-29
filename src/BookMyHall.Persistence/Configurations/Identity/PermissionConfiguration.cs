using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permission", "identity");
        builder.HasKey(x =>  x.PermissionId );
        builder.Property(x => x.PermissionId).HasDefaultValueSql("gen_random_uuid()");;
    }
}