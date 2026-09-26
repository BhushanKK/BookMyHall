using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorSubCategoryConfiguration : IEntityTypeConfiguration<VendorSubCategory>
{
    public void Configure(EntityTypeBuilder<VendorSubCategory> builder)
    {
        builder.ToTable("VendorSubCategory", "venue");
        builder.HasKey(x =>  x.VendorSubCategoryId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
