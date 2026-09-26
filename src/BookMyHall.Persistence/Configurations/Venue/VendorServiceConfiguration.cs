using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorServiceConfiguration : IEntityTypeConfiguration<VendorService>
{
    public void Configure(EntityTypeBuilder<VendorService> builder)
    {
        builder.ToTable("VendorService", "venue");
        builder.HasKey(x =>  x.VendorServiceId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
