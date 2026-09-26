using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorAvailabilityConfiguration : IEntityTypeConfiguration<VendorAvailability>
{
    public void Configure(EntityTypeBuilder<VendorAvailability> builder)
    {
        builder.ToTable("VendorAvailability", "venue");
        builder.HasKey(x =>  x.VendorAvailabilityId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
