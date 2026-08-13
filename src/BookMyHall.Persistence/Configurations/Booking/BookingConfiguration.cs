using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Persistence.Context;
public sealed class BookingConfiguration : IEntityTypeConfiguration<Bookings>
{
    public void Configure(EntityTypeBuilder<Bookings> builder)
    {
        builder.ToTable("Booking", "booking");
        builder.HasKey(x =>  x.BookingId );
    }
}
