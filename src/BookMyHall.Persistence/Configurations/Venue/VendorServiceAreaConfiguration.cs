using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorServiceAreaConfiguration : IEntityTypeConfiguration<VendorServiceArea>
{
    public void Configure(EntityTypeBuilder<VendorServiceArea> builder)
    {
        builder.ToTable("VendorServiceArea", "venue");
        builder.HasKey(x =>  x.VendorServiceAreaId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
