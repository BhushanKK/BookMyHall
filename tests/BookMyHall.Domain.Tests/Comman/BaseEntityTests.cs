using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Tests.Common;

public sealed class BaseEntityTests
{
    [Fact]
    public void BaseEntity_ShouldHaveDefaultValues()
    {
        // Arrange
        var entity = new TestBaseEntity();

        // Assert
        Assert.Null(entity.CreatedBy);
        Assert.Equal(default, entity.CreatedDate);
        Assert.Null(entity.UpdatedBy);
        Assert.Null(entity.UpdatedDate);
    }

    [Fact]
    public void BaseEntity_ShouldSetCreatedBy()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var userId = Guid.NewGuid();

        // Act
        entity.CreatedBy = userId;

        // Assert
        Assert.Equal(userId, entity.CreatedBy);
    }

    [Fact]
    public void BaseEntity_ShouldSetCreatedDate()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var createdDate = DateTimeOffset.UtcNow;

        // Act
        entity.CreatedDate = createdDate;

        // Assert
        Assert.Equal(createdDate, entity.CreatedDate);
    }

    [Fact]
    public void BaseEntity_ShouldSetUpdatedBy()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var userId = Guid.NewGuid();

        // Act
        entity.UpdatedBy = userId;

        // Assert
        Assert.Equal(userId, entity.UpdatedBy);
    }

    [Fact]
    public void BaseEntity_ShouldSetUpdatedDate()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var updatedDate = DateTimeOffset.UtcNow;

        // Act
        entity.UpdatedDate = updatedDate;

        // Assert
        Assert.Equal(updatedDate, entity.UpdatedDate);
    }

    [Fact]
    public void BaseEntity_ShouldSetAllAuditProperties()
    {
        // Arrange
        var entity = new TestBaseEntity();

        var createdBy = Guid.NewGuid();
        var createdDate = DateTimeOffset.UtcNow.AddMinutes(-10);

        var updatedBy = Guid.NewGuid();
        var updatedDate = DateTimeOffset.UtcNow;

        // Act
        entity.CreatedBy = createdBy;
        entity.CreatedDate = createdDate;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedDate = updatedDate;

        // Assert
        Assert.Equal(createdBy, entity.CreatedBy);
        Assert.Equal(createdDate, entity.CreatedDate);
        Assert.Equal(updatedBy, entity.UpdatedBy);
        Assert.Equal(updatedDate, entity.UpdatedDate);
    }

    private class TestBaseEntity : BaseEntity
    {
    }
}