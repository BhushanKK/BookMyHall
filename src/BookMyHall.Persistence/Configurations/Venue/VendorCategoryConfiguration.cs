using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorCategoryConfiguration : IEntityTypeConfiguration<VendorCategory>
{
    public void Configure(EntityTypeBuilder<VendorCategory> builder)
    {
        builder.ToTable("VendorCategory", "venue");
        builder.HasKey(x =>  x.VendorCategoryId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
