using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorConfiguration : IEntityTypeConfiguration<Vendors>
{
    public void Configure(EntityTypeBuilder<Vendors> builder)
    {
        builder.ToTable("Vendor", "venue");
        builder.HasKey(x =>  x.VendorId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
