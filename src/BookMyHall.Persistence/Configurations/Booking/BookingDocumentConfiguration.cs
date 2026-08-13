using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingDocumentConfiguration : IEntityTypeConfiguration<BookingDocument>
{
    public void Configure(EntityTypeBuilder<BookingDocument> builder)
    {
        builder.ToTable("BookingDocument", "booking");
        builder.HasKey(x =>  x.BookingDocumentId );
    }
}
