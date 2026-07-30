using BookMyHall.Domain.Review;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallReviewReplyConfiguration : IEntityTypeConfiguration<HallReviewReply>
{
    public void Configure(EntityTypeBuilder<HallReviewReply> builder)
    {
        builder.ToTable("HallReviewReply", "review");
        builder.HasKey(x =>  x.HallReviewReplyId );
        builder.Property(x => x.HallReviewReplyId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
