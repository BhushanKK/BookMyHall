using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("Amenity", "masters");
        builder.HasKey(x =>  x.AmenityId );
        builder.Property(x => x.AmenityId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
