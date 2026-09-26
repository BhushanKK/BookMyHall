using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorPackageItemConfiguration : IEntityTypeConfiguration<VendorPackageItem>
{
    public void Configure(EntityTypeBuilder<VendorPackageItem> builder)
    {
        builder.ToTable("VendorPackageItem", "venue");
        builder.HasKey(x =>  x.VendorPackageItemId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
