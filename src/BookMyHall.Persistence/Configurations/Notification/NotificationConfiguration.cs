using BookMyHall.Domain.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification", "notification");
        builder.HasKey(x =>  x.NotificationId );
        builder.Property(x => x.NotificationId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
