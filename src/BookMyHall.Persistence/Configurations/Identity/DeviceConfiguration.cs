using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Device", "identity");
        builder.HasKey(x =>  x.DeviceId );
        builder.Property(x => x.DeviceId).HasDefaultValueSql("gen_random_uuid()");;
    }
}