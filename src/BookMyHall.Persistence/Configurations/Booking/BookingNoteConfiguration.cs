using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingNoteConfiguration : IEntityTypeConfiguration<BookingNote>
{
    public void Configure(EntityTypeBuilder<BookingNote> builder)
    {
        builder.ToTable("BookingNote", "booking");
        builder.HasKey(x =>  x.BookingNoteId );
        builder.Property(x => x.BookingNoteId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
