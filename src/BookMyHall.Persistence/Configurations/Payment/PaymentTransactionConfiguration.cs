using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransaction", "payment");
        builder.HasKey(x =>  x.PaymentTransactionId );
        builder.Property(x => x.PaymentTransactionId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
