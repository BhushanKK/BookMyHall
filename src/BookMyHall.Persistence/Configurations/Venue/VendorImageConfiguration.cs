using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorImageConfiguration : IEntityTypeConfiguration<VendorImage>
{
    public void Configure(EntityTypeBuilder<VendorImage> builder)
    {
        builder.ToTable("VendorImage", "venue");
        builder.HasKey(x =>  x.VendorImageId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
