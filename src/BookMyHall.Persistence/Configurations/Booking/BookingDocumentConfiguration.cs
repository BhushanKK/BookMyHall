using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingDocumentConfiguration : IEntityTypeConfiguration<BookingDocument>
{
    public void Configure(EntityTypeBuilder<BookingDocument> builder)
    {
        builder.ToTable("BookingDocument", "booking");
        builder.HasKey(x =>  x.BookingDocumentId );
        builder.Property(x => x.BookingDocumentId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
