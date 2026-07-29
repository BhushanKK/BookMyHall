using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingCancellationConfiguration : IEntityTypeConfiguration<BookingCancellation>
{
    public void Configure(EntityTypeBuilder<BookingCancellation> builder)
    {
        builder.ToTable("BookingCancellation", "booking");
        builder.HasKey(x =>  x.BookingCancellationId );
        builder.Property(x => x.BookingCancellationId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
