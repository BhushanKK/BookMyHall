using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class PaymentModeConfiguration : IEntityTypeConfiguration<PaymentMode>
{
    public void Configure(EntityTypeBuilder<PaymentMode> builder)
    {
        builder.ToTable("PaymentMode", "masters");
        builder.HasKey(x =>  x.PaymentModeId );
        builder.Property(x => x.PaymentModeId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
