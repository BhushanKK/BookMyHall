using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingEventConfiguration : IEntityTypeConfiguration<BookingEvent>
{
    public void Configure(EntityTypeBuilder<BookingEvent> builder)
    {
        builder.ToTable("BookingEvent", "booking");
        builder.HasKey(x =>  x.BookingEventId );
        builder.Property(x => x.BookingEventId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
