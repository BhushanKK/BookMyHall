using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorDocumentConfiguration : IEntityTypeConfiguration<VendorDocument>
{
    public void Configure(EntityTypeBuilder<VendorDocument> builder)
    {
        builder.ToTable("VendorDocument", "venue");
        builder.HasKey(x =>  x.VendorDocumentId );
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
