using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder.ToTable("NotificationRecipient", "notification");
        builder.HasKey(x =>  x.NotificationRecipientId );
        builder.Property(x => x.NotificationRecipientId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
