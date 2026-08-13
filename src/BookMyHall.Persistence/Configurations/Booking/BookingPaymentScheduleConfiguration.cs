using BookMyHall.Domain.Booking;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingPaymentScheduleConfiguration : IEntityTypeConfiguration<BookingPaymentSchedule>
{
    public void Configure(EntityTypeBuilder<BookingPaymentSchedule> builder)
    {
        builder.ToTable("BookingPaymentSchedule", "booking");
        builder.HasKey(x =>  x.BookingPaymentScheduleId );
    }
}
