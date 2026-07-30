using BookMyHall.Domain.Audit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class ApiRequestLogConfiguration : IEntityTypeConfiguration<ApiRequestLog>
{
    public void Configure(EntityTypeBuilder<ApiRequestLog> builder)
    {
        builder.ToTable("ApiRequestLog", "audit");
        builder.HasKey(x =>  x.ApiRequestLogId );
        builder.Property(x => x.ApiRequestLogId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
