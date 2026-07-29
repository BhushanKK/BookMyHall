using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Service", "masters");
        builder.HasKey(x =>  x.ServiceId );
        builder.Property(x => x.ServiceId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
