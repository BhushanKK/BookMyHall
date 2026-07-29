using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class BookingStatusHistoryConfiguration : IEntityTypeConfiguration<BookingStatusHistory>
{
    public void Configure(EntityTypeBuilder<BookingStatusHistory> builder)
    {
        builder.ToTable("BookingStatusHistory", "booking");
        builder.HasKey(x =>  x.BookingStatusHistoryId );
        builder.Property(x => x.BookingStatusHistoryId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
