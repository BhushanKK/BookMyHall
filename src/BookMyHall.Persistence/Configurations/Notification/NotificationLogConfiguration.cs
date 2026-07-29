using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLog", "notification");
        builder.HasKey(x =>  x.NotificationLogId );
        builder.Property(x => x.NotificationLogId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
