using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("District", "masters");
        builder.HasKey(x =>  x.DistrictId );
        builder.Property(x => x.DistrictId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
