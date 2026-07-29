using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallReviewConfiguration : IEntityTypeConfiguration<HallReview>
{
    public void Configure(EntityTypeBuilder<HallReview> builder)
    {
        builder.ToTable("HallReview", "review");
        builder.HasKey(x =>  x.HallReviewId );
        builder.Property(x => x.HallReviewId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
