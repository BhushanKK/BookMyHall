using BookMyHall.Domain.Payments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment", "payment");
        builder.HasKey(x =>  x.PaymentId );
        builder.Property(x => x.PaymentId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
