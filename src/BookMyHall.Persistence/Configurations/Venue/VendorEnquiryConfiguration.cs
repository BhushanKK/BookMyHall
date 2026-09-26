using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorEnquiryConfiguration : IEntityTypeConfiguration<VendorEnquiry>
{
    public void Configure(EntityTypeBuilder<VendorEnquiry> builder)
    {
        builder.ToTable("VendorEnquiry", "venue");
        builder.HasKey(x =>  x.VendorEnquiryId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
