using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoice", "payment");
        builder.HasKey(x =>  x.InvoiceId );
        builder.Property(x => x.InvoiceId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
