using BookMyHall.Domain.Venue;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallBlockConfiguration : IEntityTypeConfiguration<HallBlock>
{
    public void Configure(EntityTypeBuilder<HallBlock> builder)
    {
        builder.ToTable("HallBlock", "venue");
        builder.HasKey(x =>  x.HallBlockId );
        builder.Property(x => x.HallBlockId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
