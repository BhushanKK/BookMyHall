using BookMyHall.Domain.Notifications;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreference", "notification");
        builder.HasKey(x =>  x.NotificationPreferenceId );
        builder.Property(x => x.NotificationPreferenceId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
