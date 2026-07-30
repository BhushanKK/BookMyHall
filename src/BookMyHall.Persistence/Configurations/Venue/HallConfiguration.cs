using BookMyHall.Domain.Venue;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallConfiguration : IEntityTypeConfiguration<Hall>
{
    public void Configure(EntityTypeBuilder<Hall> builder)
    {
        builder.ToTable("Hall", "masters");
        builder.HasKey(x =>  x.HallId );
        builder.Property(x => x.HallId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
