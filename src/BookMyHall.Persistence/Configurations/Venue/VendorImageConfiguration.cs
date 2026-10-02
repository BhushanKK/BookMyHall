using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class VendorImageConfiguration : IEntityTypeConfiguration<VendorImage>
{
    public void Configure(EntityTypeBuilder<VendorImage> builder)
    {
        builder.ToTable("VendorImage", "venue");

        builder.HasKey(x => x.VendorImageId);

        builder.Property(x => x.VendorImageId)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.VendorId)
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .IsRequired();

        builder.Property(x => x.ThumbnailUrl);

        builder.Property(x => x.DisplayOrder)
            .HasDefaultValue(1);

        builder.Property(x => x.IsCoverImage)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.Vendor)
            .WithMany()
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VendorId);

        builder.HasIndex(x => new
        {
            x.VendorId,
            x.DisplayOrder
        })
        .HasFilter("\"IsActive\" = TRUE");

        builder.HasIndex(x => x.VendorId)
            .IsUnique()
            .HasFilter("\"IsCoverImage\" = TRUE AND \"IsActive\" = TRUE");
    }
}