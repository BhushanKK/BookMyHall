using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationProviderConfiguration : IEntityTypeConfiguration<NotificationProvider>
{
    public void Configure(EntityTypeBuilder<NotificationProvider> builder)
    {
        builder.ToTable("NotificationProvider", "notification");
        builder.HasKey(x =>  x.NotificationProviderId );
        builder.Property(x => x.NotificationProviderId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
