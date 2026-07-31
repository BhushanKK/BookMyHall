using BookMyHall.Domain.Common;
using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Security;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    private readonly ICurrentUser _currentUser;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var userId = _currentUser.UserId;

        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e =>
                e.State == EntityState.Added ||
                e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedDate = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:

                    entry.Property(x => x.CreatedBy).IsModified = false;
                    entry.Property(x => x.CreatedDate).IsModified = false;

                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    break;
            }
        }
    }
}