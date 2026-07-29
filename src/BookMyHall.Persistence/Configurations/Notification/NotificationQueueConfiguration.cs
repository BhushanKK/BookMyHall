using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationQueueConfiguration : IEntityTypeConfiguration<NotificationQueue>
{
    public void Configure(EntityTypeBuilder<NotificationQueue> builder)
    {
        builder.ToTable("NotificationQueue", "notification");
        builder.HasKey(x =>  x.NotificationQueueId );
        builder.Property(x => x.NotificationQueueId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
