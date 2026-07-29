using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class CancellationPolicyConfiguration : IEntityTypeConfiguration<CancellationPolicy>
{
    public void Configure(EntityTypeBuilder<CancellationPolicy> builder)
    {
        builder.ToTable("CancellationPolicy", "masters");
        builder.HasKey(x =>  x.CancellationPolicyId );
        builder.Property(x => x.CancellationPolicyId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
