using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Booking", "booking");
        builder.HasKey(x =>  x.BookingId );
        builder.Property(x => x.BookingId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
