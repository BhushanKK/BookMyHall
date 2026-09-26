using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorReviewConfiguration : IEntityTypeConfiguration<VendorReview>
{
    public void Configure(EntityTypeBuilder<VendorReview> builder)
    {
        builder.ToTable("VendorReview", "venue");
        builder.HasKey(x =>  x.VendorReviewId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
