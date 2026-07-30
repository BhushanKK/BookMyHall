using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallPricingConfiguration : IEntityTypeConfiguration<HallPricing>
{
    public void Configure(EntityTypeBuilder<HallPricing> builder)
    {
        builder.ToTable("HallPricing", "venue");
        builder.HasKey(x =>  x.HallPricingId );
        builder.Property(x => x.HallPricingId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
