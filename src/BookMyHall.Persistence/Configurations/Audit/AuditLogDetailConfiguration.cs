using BookMyHall.Domain.Audit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class AuditLogDetailConfiguration : IEntityTypeConfiguration<AuditLogDetail>
{
    public void Configure(EntityTypeBuilder<AuditLogDetail> builder)
    {
        builder.ToTable("AuditLogDetail", "audit");
        builder.HasKey(x =>  x.AuditLogDetailId );
        builder.Property(x => x.AuditLogDetailId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
