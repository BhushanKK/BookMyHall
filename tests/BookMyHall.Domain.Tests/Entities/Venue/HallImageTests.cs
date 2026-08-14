using BookMyHall.Domain.Venue;
using FluentAssertions;

namespace BookMyHall.Domain.Tests.Venue;

public sealed class HallImageTests
{
    [Fact]
    public void HallImage_Should_Create_Using_Private_Constructor()
    {
        // Arrange
        var constructor = typeof(HallImage)
            .GetConstructor(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);

        // Act
        var image = constructor!.Invoke(null);

        // Assert
        image.Should().NotBeNull();
        image.Should().BeOfType<HallImage>();
    }

    [Fact]
    public void HallImage_Should_Create_With_Valid_Values()
    {
        // Arrange
        var hallImageId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();

        // Act
        var image = new HallImage(
            hallImageId,
            hallId,
            "/images/hall/main.jpg",
            "/images/hall/thumb.jpg",
            1,
            true,
            createdBy);

        // Assert
        image.HallImageId.Should().Be(hallImageId);
        image.HallId.Should().Be(hallId);
        image.ImageUrl.Should().Be("/images/hall/main.jpg");
        image.ThumbnailUrl.Should().Be("/images/hall/thumb.jpg");
        image.DisplayOrder.Should().Be(1);
        image.IsCoverImage.Should().BeTrue();
        image.IsActive.Should().BeTrue();
        image.CreatedBy.Should().Be(createdBy);
        image.CreatedDate.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void HallImage_Should_Not_Be_CoverImage_By_Default()
    {
        // Arrange
        var hallImageId = Guid.NewGuid();
        var hallId = Guid.NewGuid();

        // Act
        var image = new HallImage(
            hallImageId,
            hallId,
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Assert
        image.IsCoverImage.Should().BeFalse();
    }

    [Fact]
    public void HallImage_Should_Be_Active_By_Default()
    {
        // Arrange
        var hallImageId = Guid.NewGuid();
        var hallId = Guid.NewGuid();

        // Act
        var image = new HallImage(
            hallImageId,
            hallId,
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Assert
        image.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallImage_Should_Assign_HallImageId()
    {
        // Arrange
        var hallImageId = Guid.NewGuid();

        // Act
        var image = new HallImage(
            hallImageId,
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Assert
        image.HallImageId.Should().Be(hallImageId);
    }

    [Fact]
    public void HallImage_Should_Set_HallId()
    {
        // Arrange
        var hallId = Guid.NewGuid();

        // Act
        var image = new HallImage(
            Guid.NewGuid(),
            hallId,
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Assert
        image.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallImage_Should_Set_ImageUrl()
    {
        // Arrange
        var imageUrl = "/images/hall/main.jpg";

        // Act
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            imageUrl,
            null,
            1,
            false,
            null);

        // Assert
        image.ImageUrl.Should().Be(imageUrl);
    }

    [Fact]
    public void HallImage_Should_Set_ThumbnailUrl()
    {
        // Arrange
        var thumbnailUrl = "/images/hall/thumb.jpg";

        // Act
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            thumbnailUrl,
            1,
            false,
            null);

        // Assert
        image.ThumbnailUrl.Should().Be(thumbnailUrl);
    }

    [Fact]
    public void HallImage_Should_Set_DisplayOrder()
    {
        // Arrange
        var displayOrder = 2;

        // Act
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            displayOrder,
            false,
            null);

        // Assert
        image.DisplayOrder.Should().Be(displayOrder);
    }

    [Fact]
    public void HallImage_Should_Set_CreatedBy()
    {
        // Arrange
        var createdBy = Guid.NewGuid();

        // Act
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            false,
            createdBy);

        // Assert
        image.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    public void SetCoverImage_Should_Update_IsCoverImage()
    {
        // Arrange
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Act
        image.SetCoverImage(true);

        // Assert
        image.IsCoverImage.Should().BeTrue();
    }

    [Fact]
    public void SetCoverImage_Should_Update_IsCoverImage_To_False()
    {
        // Arrange
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            true,
            null);

        // Act
        image.SetCoverImage(false);

        // Assert
        image.IsCoverImage.Should().BeFalse();
    }

    [Fact]
    public void SetActive_Should_Update_IsActive()
    {
        // Arrange
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Act
        image.SetActive(false);

        // Assert
        image.IsActive.Should().BeFalse();
    }

    [Fact]
    public void SetActive_Should_Activate_Image()
    {
        // Arrange
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        image.SetActive(false);

        // Act
        image.SetActive(true);

        // Assert
        image.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateDisplayOrder_Should_Update_DisplayOrder()
    {
        // Arrange
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            null,
            1,
            false,
            null);

        // Act
        image.UpdateDisplayOrder(5);

        // Assert
        image.DisplayOrder.Should().Be(5);
    }

    [Fact]
    public void Update_Should_Update_Image_Properties()
    {
        // Arrange
        var image = new HallImage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/images/hall/main.jpg",
            "/images/hall/thumb.jpg",
            1,
            false,
            null);

        // Act
        image.Update(
            "/images/hall/updated.jpg",
            "/images/hall/updated-thumb.jpg",
            3,
            true);

        // Assert
        image.ImageUrl.Should().Be("/images/hall/updated.jpg");
        image.ThumbnailUrl.Should().Be("/images/hall/updated-thumb.jpg");
        image.DisplayOrder.Should().Be(3);
        image.IsCoverImage.Should().BeTrue();
    }
}