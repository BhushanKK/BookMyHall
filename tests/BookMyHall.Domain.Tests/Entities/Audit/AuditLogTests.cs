using FluentAssertions;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Domain.Tests.Entities.Audit;

public sealed class AuditLogTests
{
    [Fact]
    public void AuditLog_Should_Assign_AuditLogId()
    {
        var auditLog = new AuditLog();
        var id = Guid.NewGuid();

        auditLog.AuditLogId = id;

        auditLog.AuditLogId.Should().Be(id);
    }

    [Fact]
    public void AuditLog_Should_Assign_TableName()
    {
        var auditLog = new AuditLog();

        auditLog.TableName = "Users";

        auditLog.TableName.Should().Be("Users");
    }

    [Fact]
    public void AuditLog_Should_Assign_RecordId()
    {
        var auditLog = new AuditLog();
        var recordId = Guid.NewGuid();

        auditLog.RecordId = recordId;

        auditLog.RecordId.Should().Be(recordId);
    }

    [Fact]
    public void AuditLog_Should_Assign_Operation()
    {
        var auditLog = new AuditLog();

        auditLog.Operation = "INSERT";

        auditLog.Operation.Should().Be("INSERT");
    }

    [Fact]
    public void AuditLog_Should_Assign_UserId()
    {
        var auditLog = new AuditLog();
        var userId = Guid.NewGuid();

        auditLog.UserId = userId;

        auditLog.UserId.Should().Be(userId);
    }

    [Fact]
    public void AuditLog_Should_Assign_IpAddress()
    {
        var auditLog = new AuditLog();

        auditLog.IpAddress = "192.168.1.10";

        auditLog.IpAddress.Should().Be("192.168.1.10");
    }

    [Fact]
    public void AuditLog_Should_Assign_UserAgent()
    {
        var auditLog = new AuditLog();

        auditLog.UserAgent = "Mozilla/5.0";

        auditLog.UserAgent.Should().Be("Mozilla/5.0");
    }

    [Fact]
    public void AuditLog_Should_Assign_CorrelationId()
    {
        var auditLog = new AuditLog();
        var correlationId = Guid.NewGuid();

        auditLog.CorrelationId = correlationId;

        auditLog.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void AuditLog_Should_Assign_All_Properties()
    {
        var auditLogId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var auditLog = new AuditLog
        {
            AuditLogId = auditLogId,
            TableName = "Users",
            RecordId = recordId,
            Operation = "UPDATE",
            UserId = userId,
            IpAddress = "192.168.1.10",
            UserAgent = "Mozilla/5.0",
            CorrelationId = correlationId
        };

        auditLog.AuditLogId.Should().Be(auditLogId);
        auditLog.TableName.Should().Be("Users");
        auditLog.RecordId.Should().Be(recordId);
        auditLog.Operation.Should().Be("UPDATE");
        auditLog.UserId.Should().Be(userId);
        auditLog.IpAddress.Should().Be("192.168.1.10");
        auditLog.UserAgent.Should().Be("Mozilla/5.0");
        auditLog.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void AuditLog_Should_Have_Default_Values()
    {
        var auditLog = new AuditLog();

        auditLog.AuditLogId.Should().Be(Guid.Empty);
        auditLog.TableName.Should().BeEmpty();
        auditLog.RecordId.Should().Be(Guid.Empty);
        auditLog.Operation.Should().BeEmpty();
        auditLog.UserId.Should().Be(Guid.Empty);
        auditLog.IpAddress.Should().BeEmpty();
        auditLog.UserAgent.Should().BeEmpty();
        auditLog.CorrelationId.Should().Be(Guid.Empty);
    }
}