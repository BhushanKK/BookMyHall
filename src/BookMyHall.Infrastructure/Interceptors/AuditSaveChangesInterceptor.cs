using System.Text.Json;

using BookMyHall.Application.Abstractions.Audit;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Domain.Audit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookMyHall.Infrastructure.Persistence.Interceptors;

public sealed class AuditSaveChangesInterceptor(
    ICurrentUser currentUser, IAuditRequestContext auditRequestContext)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditLogs(DbContext? context)
    {
        if (context is null)
            return;

        var entries = context.ChangeTracker
            .Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(entry => !IsAuditEntity(entry.Entity))
            .ToList();

        if (entries.Count == 0)
            return;

        foreach (var entry in entries)
        {
            var auditLog = CreateAuditLog(entry);

            if (auditLog is null)
                continue;

            context.Set<AuditLog>() .Add(auditLog);
            AddAuditLogDetails(context, entry, auditLog.AuditLogId);
        }
    }

    private AuditLog? CreateAuditLog(EntityEntry entry)
    {
        var recordId = GetRecordId(entry);

        if (recordId is null)
            return null;

        var tableName = entry.Metadata.GetTableName();

        if (string.IsNullOrWhiteSpace(tableName))
            return null;

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

    private static void AddAuditLogDetails(DbContext context, EntityEntry entry, Guid auditLogId)
    {
        foreach (var property in entry.Properties)
        {
            if (ShouldSkipProperty(property))
                continue;

            if (entry.State == EntityState.Modified && !property.IsModified)
                continue;

            var detail = new AuditLogDetail
            {
                AuditLogDetailId = Guid.NewGuid(),
                AuditLogId = auditLogId,
                ColumnName = property.Metadata.GetColumnName() ?? property.Metadata.Name,

                OldValue = entry.State == EntityState.Added
                        ? null
                        : SerializeValue(property.OriginalValue),

                NewValue =
                    entry.State == EntityState.Deleted
                        ? null 
                        : SerializeValue(property.CurrentValue)
            };

            context.Set<AuditLogDetail>().Add(detail);
        }
    }

    private static Guid? GetRecordId(
        EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();

        if (primaryKey is null || primaryKey.Properties.Count != 1)
            return null;

        var keyProperty = primaryKey.Properties[0];
        var property = entry.Property(keyProperty.Name);
        var value = entry.State == EntityState.Deleted
                ? property.OriginalValue
                : property.CurrentValue;

        return value is Guid id && id != Guid.Empty ? id : null;
    }

    private static string GetOperation(EntityState state)
    {
        return state switch
        {
            EntityState.Added => "INSERT",
            EntityState.Modified => "UPDATE",
            EntityState.Deleted => "DELETE",
            _ => string.Empty
        };
    }

    private static bool ShouldSkipProperty(PropertyEntry property)
    {
        if (property.Metadata.IsPrimaryKey())
            return true;

        return SensitiveProperties.Contains(property.Metadata.Name);
    }

    private static string? SerializeValue(object? value)
    {
        if (value is null)
            return null;

        return value switch
        {
            string text => text,

            DateTime dateTime => dateTime.ToString("O"),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
            _ => JsonSerializer.Serialize(value)
        };
    }

    private static bool IsAuditEntity(object entity)
        => entity is AuditLog or AuditLogDetail or ApiRequestLog or ErrorLog;

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
}