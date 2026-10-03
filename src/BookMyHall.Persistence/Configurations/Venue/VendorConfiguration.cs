using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendor", "venue");
        builder.HasKey(x =>  x.VendorId );
        builder.Property(x => x.LogoUrl).HasColumnType("text");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
