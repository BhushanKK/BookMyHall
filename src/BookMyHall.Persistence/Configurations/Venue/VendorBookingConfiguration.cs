using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorBookingConfiguration : IEntityTypeConfiguration<VendorBooking>
{
    public void Configure(EntityTypeBuilder<VendorBooking> builder)
    {
        builder.ToTable("VendorBooking", "venue");
        builder.HasKey(x =>  x.VendorBookingId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
