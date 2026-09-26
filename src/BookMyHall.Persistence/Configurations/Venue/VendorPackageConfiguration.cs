using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorPackageConfiguration : IEntityTypeConfiguration<VendorPackage>
{
    public void Configure(EntityTypeBuilder<VendorPackage> builder)
    {
        builder.ToTable("VendorPackage", "venue");
        builder.HasKey(x =>  x.VendorPackageId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
