using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationQueue> NotificationQueues => Set<NotificationQueue>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<NotificationProvider> NotificationProviders => Set<NotificationProvider>();
}