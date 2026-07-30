using FluentAssertions;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Domain.Tests.Entities.Audit;

public sealed class AuditLogDetailTests
{
    [Fact]
    public void AuditLogDetail_Should_Assign_AuditLogDetailId()
    {
        var auditLogDetail = new AuditLogDetail();
        var id = Guid.NewGuid();

        auditLogDetail.AuditLogDetailId = id;

        auditLogDetail.AuditLogDetailId.Should().Be(id);
    }

    [Fact]
    public void AuditLogDetail_Should_Assign_AuditLogId()
    {
        var auditLogDetail = new AuditLogDetail();
        var auditLogId = Guid.NewGuid();

        auditLogDetail.AuditLogId = auditLogId;

        auditLogDetail.AuditLogId.Should().Be(auditLogId);
    }

    [Fact]
    public void AuditLogDetail_Should_Assign_ColumnName()
    {
        var auditLogDetail = new AuditLogDetail();

        auditLogDetail.ColumnName = "FirstName";

        auditLogDetail.ColumnName.Should().Be("FirstName");
    }

    [Fact]
    public void AuditLogDetail_Should_Assign_OldValue()
    {
        var auditLogDetail = new AuditLogDetail();

        auditLogDetail.OldValue = "John";

        auditLogDetail.OldValue.Should().Be("John");
    }

    [Fact]
    public void AuditLogDetail_Should_Assign_NewValue()
    {
        var auditLogDetail = new AuditLogDetail();

        auditLogDetail.NewValue = "Johnny";

        auditLogDetail.NewValue.Should().Be("Johnny");
    }

    [Fact]
    public void AuditLogDetail_Should_Assign_All_Properties()
    {
        var auditLogDetailId = Guid.NewGuid();
        var auditLogId = Guid.NewGuid();

        var auditLogDetail = new AuditLogDetail
        {
            AuditLogDetailId = auditLogDetailId,
            AuditLogId = auditLogId,
            ColumnName = "FirstName",
            OldValue = "John",
            NewValue = "Johnny"
        };

        auditLogDetail.AuditLogDetailId.Should().Be(auditLogDetailId);
        auditLogDetail.AuditLogId.Should().Be(auditLogId);
        auditLogDetail.ColumnName.Should().Be("FirstName");
        auditLogDetail.OldValue.Should().Be("John");
        auditLogDetail.NewValue.Should().Be("Johnny");
    }

    [Fact]
    public void AuditLogDetail_Should_Have_Default_Values()
    {
        var auditLogDetail = new AuditLogDetail();

        auditLogDetail.AuditLogDetailId.Should().Be(Guid.Empty);
        auditLogDetail.AuditLogId.Should().Be(Guid.Empty);
        auditLogDetail.ColumnName.Should().BeEmpty();
        auditLogDetail.OldValue.Should().BeNull();
        auditLogDetail.NewValue.Should().BeNull();
    }
}