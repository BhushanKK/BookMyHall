
using System.Text.Json;
using BookMyHall.Application.Abstractions.Audit;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Domain.Audit;
using BookMyHall.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookMyHall.Persistence.Interceptors;
public sealed class AuditSaveChangesInterceptor(ICurrentUser currentUser,IAuditRequestContext auditRequestContext)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
    InterceptionResult<int> result)
    {
        ProcessChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ProcessChanges(eventData.Context);
        return base.SavingChangesAsync(eventData,result,cancellationToken);
    }

    private void ProcessChanges(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var entries = context.ChangeTracker
            .Entries()
            .Where(IsTrackedEntry)
            .Where(entry => !IsAuditEntity(entry.Entity))
            .ToList();

        if (entries.Count == 0)
        {
            return;
        }

        SetAuditFields(entries);
        CreateAuditLogs(context, entries);
    }

    private void SetAuditFields(
        IReadOnlyCollection<EntityEntry> entries)
    {
        var now = DateTimeOffset.UtcNow;
        var userId = currentUser.UserId;

        foreach (var entry in entries)
        {
            if (entry.Entity is not BaseEntity entity)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreatedAuditFields(entity, now, userId);
                    break;

                case EntityState.Modified:
                    SetUpdatedAuditFields(entity, now, userId);
                    break;
            }
        }
    }

    private static void SetCreatedAuditFields(BaseEntity entity, DateTimeOffset now,Guid? userId)
    {
        entity.CreatedDate = now;
        entity.CreatedBy = userId;
    }

    private static void SetUpdatedAuditFields(BaseEntity entity,DateTimeOffset now,Guid? userId)
    {
        entity.UpdatedDate = now;
        entity.UpdatedBy = userId;
    }

    private void CreateAuditLogs(DbContext context,IReadOnlyCollection<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            var auditLog = CreateAuditLog(entry);
            if (auditLog is null)
            {
                continue;
            }

            context.Set<AuditLog>().Add(auditLog);
            AddAuditLogDetails(context,entry,auditLog.AuditLogId);
        }
    }

    private AuditLog? CreateAuditLog(EntityEntry entry)
    {
        var recordId = GetRecordId(entry);
        var tableName = entry.Metadata.GetTableName();

        if (recordId is null || string.IsNullOrWhiteSpace(tableName))
        {
            return null;
        }

        return new AuditLog
        {
            AuditLogId = Guid.NewGuid(),
            TableName = tableName,
            RecordId = recordId.Value,
            Operation = GetOperation(entry.State),
            UserId = currentUser.UserId,
            IpAddress = auditRequestContext.IpAddress ?? string.Empty,
            UserAgent = auditRequestContext.UserAgent ?? string.Empty,
            CorrelationId = auditRequestContext.CorrelationId
        };
    }

    private static void AddAuditLogDetails(DbContext context,EntityEntry entry,Guid auditLogId)
    {
        foreach (var property in entry.Properties)
        {
            if (ShouldSkipProperty(property) || IsUnchangedProperty(entry, property))
            {
                continue;
            }

            var detail = new AuditLogDetail
            {
                AuditLogDetailId = Guid.NewGuid(),
                AuditLogId = auditLogId,
                ColumnName = GetColumnName(property),
                OldValue = GetOldValue(entry, property),
                NewValue = GetNewValue(entry, property)
            };

            context.Set<AuditLogDetail>().Add(detail);
        }
    }

    private static bool IsTrackedEntry(EntityEntry entry)
    {
        return entry.State is
            EntityState.Added or
            EntityState.Modified or
            EntityState.Deleted;
    }

    private static bool IsUnchangedProperty(EntityEntry entry,PropertyEntry property)
    {
        return entry.State == EntityState.Modified && !property.IsModified;
    }

    private static string GetColumnName(PropertyEntry property)
    {
        return property.Metadata.GetColumnName() ?? property.Metadata.Name;
    }

    private static string? GetOldValue(EntityEntry entry, PropertyEntry property)
    {
        return entry.State == EntityState.Added ? null: SerializeValue(property.OriginalValue);
    }

    private static string? GetNewValue(EntityEntry entry,PropertyEntry property)
    {
        return entry.State == EntityState.Deleted? null: SerializeValue(property.CurrentValue);
    }

    private static Guid? GetRecordId(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();

        if (primaryKey is null || primaryKey.Properties.Count != 1)
        {
            return null;
        }

        var keyProperty = primaryKey.Properties[0];
        var property = entry.Property(keyProperty.Name);

        var value = entry.State == EntityState.Deleted
            ? property.OriginalValue
            : property.CurrentValue;

        return value is Guid id && id != Guid.Empty
            ? id
            : null;
    }

    private static string GetOperation(EntityState state)
    {
        return state switch
        {
            EntityState.Added => AuditOperations.Insert,
            EntityState.Modified => AuditOperations.Update,
            EntityState.Deleted => AuditOperations.Delete,
            _ => string.Empty
        };
    }

    private static bool ShouldSkipProperty(PropertyEntry property)
    {
        return property.Metadata.IsPrimaryKey() ||
               SensitiveProperties.Contains(property.Metadata.Name);
    }

    private static string? SerializeValue(object? value)
    {
        if (value is null)
        {
            return null;
        }

        return value switch
        {
            string text => text,
            DateTime dateTime => dateTime.ToString("O"),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
            _ => JsonSerializer.Serialize(value)
        };
    }

    private static bool IsAuditEntity(object entity)
    {
        return entity is
            AuditLog or
            AuditLogDetail or
            ApiRequestLog or
            ErrorLog;
    }

    private static readonly HashSet<string> SensitiveProperties =
    [
        "Password",
        "PasswordHash",
        "RefreshToken",
        "RefreshTokenHash",
        "Token",
        "TokenHash",
        "OTP",
        "OTPHash",
        "Secret"
    ];

    private static class AuditOperations
    {
        public const string Insert = "INSERT";
        public const string Update = "UPDATE";
        public const string Delete = "DELETE";
    }
}

