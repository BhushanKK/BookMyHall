using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", "identity");
        builder.HasKey(x => x.RoleId);
        builder.Property(x => x.RoleId)
            .HasDefaultValueSql("gen_random_uuid()");
    }
}