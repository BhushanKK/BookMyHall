using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplate", "notification");
        builder.HasKey(x =>  x.NotificationTemplateId );
        builder.Property(x => x.NotificationTemplateId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
