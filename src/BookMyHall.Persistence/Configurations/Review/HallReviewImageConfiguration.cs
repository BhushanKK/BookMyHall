using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallReviewImageConfiguration : IEntityTypeConfiguration<HallReviewImage>
{
    public void Configure(EntityTypeBuilder<HallReviewImage> builder)
    {
        builder.ToTable("HallReviewImage", "review");
        builder.HasKey(x =>  x.HallReviewImageId );
        builder.Property(x => x.HallReviewImageId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
