using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facility", "masters");
        builder.HasKey(x =>  x.FacilityId );
        builder.Property(x => x.FacilityId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
