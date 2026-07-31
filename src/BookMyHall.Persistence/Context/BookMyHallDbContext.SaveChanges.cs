using BookMyHall.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var userId = _currentUser.UserId;

        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry =>
                entry.State == EntityState.Added ||
                entry.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedDate = utcNow;

                    entry.Entity.UpdatedBy = null;
                    entry.Entity.UpdatedDate = null;

                    break;

                case EntityState.Modified:

                    // Prevent Created fields from being updated
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    entry.Property(x => x.CreatedDate).IsModified = false;

                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedDate = utcNow;

                    break;
            }
        }
    }
}