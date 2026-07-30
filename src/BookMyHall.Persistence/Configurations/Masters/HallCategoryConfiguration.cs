using BookMyHall.Domain.Venue;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallCategoryConfiguration : IEntityTypeConfiguration<HallCategory>
{
    public void Configure(EntityTypeBuilder<HallCategory> builder)
    {
        builder.ToTable("HallCategory", "masters");
        builder.HasKey(x =>  x.HallCategoryId );
        builder.Property(x => x.HallCategoryId).HasDefaultValueSql("gen_random_uuid()");;
    }
}