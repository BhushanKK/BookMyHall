using BookMyHall.Domain.Venue;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallImageConfiguration : IEntityTypeConfiguration<HallImage>
{
    public void Configure(EntityTypeBuilder<HallImage> builder)
    {
        builder.ToTable("HallImage", "venue");
        builder.HasKey(x =>  x.HallImageId );
        builder.Property(x => x.HallImageId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
