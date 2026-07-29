using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class PaymentWebhookConfiguration : IEntityTypeConfiguration<PaymentWebhook>
{
    public void Configure(EntityTypeBuilder<PaymentWebhook> builder)
    {
        builder.ToTable("PaymentWebhook", "payment");
        builder.HasKey(x =>  x.PaymentWebhookId );
        builder.Property(x => x.PaymentWebhookId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
