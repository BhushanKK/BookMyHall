using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingChecklistConfiguration : IEntityTypeConfiguration<BookingChecklist>
{
    public void Configure(EntityTypeBuilder<BookingChecklist> builder)
    {
        builder.ToTable("BookingChecklist", "booking");
        builder.HasKey(x =>  x.BookingChecklistId );
        builder.Property(x => x.BookingChecklistId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
