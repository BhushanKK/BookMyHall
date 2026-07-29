using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refund", "payment");
        builder.HasKey(x =>  x.RefundId );
        builder.Property(x => x.RefundId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
