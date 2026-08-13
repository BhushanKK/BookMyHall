using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingTimelineConfiguration : IEntityTypeConfiguration<BookingTimeline>
{
    public void Configure(EntityTypeBuilder<BookingTimeline> builder)
    {
        builder.ToTable("BookingTimeline", "booking");
        builder.HasKey(x =>  x.BookingTimelineId );
    }
}
