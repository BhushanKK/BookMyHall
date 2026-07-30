using FluentAssertions;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Domain.Tests.Venue;

public sealed class HallImageTests
{
    [Fact]
    public void HallImage_Should_Be_Inactive_By_Default()
    {
        var image = new HallImage();
        image.IsActive.Should().BeFalse();
    }

    [Fact]
    public void HallImage_Should_Be_CoverImage_False_By_Default()
    {
        var image = new HallImage();
        image.IsCoverImage.Should().BeFalse();
    }

    [Fact]
    public void HallImage_Should_Assign_HallImageId()
    {
        var image = new HallImage();
        var id = Guid.NewGuid();
        image.HallImageId = id;
        image.HallImageId.Should().Be(id);
    }

    [Fact]
    public void HallImage_Should_Assign_HallId()
    {
        var image = new HallImage();
        var hallId = Guid.NewGuid();
        image.HallId = hallId;
        image.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallImage_Should_Assign_ImagePath()
    {
        var image = new HallImage();
        image.ImagePath = "/images/hall/main.jpg";
        image.ImagePath.Should().Be("/images/hall/main.jpg");
    }

    [Fact]
    public void HallImage_Should_Assign_ThumbnailUrl()
    {
        var image = new HallImage();
        image.ThumbnailUrl = "/images/hall/thumb.jpg";
        image.ThumbnailUrl.Should().Be("/images/hall/thumb.jpg");
    }

    [Fact]
    public void HallImage_Should_Assign_DisplayOrder()
    {
        var image = new HallImage();
        image.DisplayOrder = "1";
        image.DisplayOrder.Should().Be("1");
    }

    [Fact]
    public void HallImage_Should_Assign_IsCoverImage()
    {
        var image = new HallImage();
        image.IsCoverImage = true;
        image.IsCoverImage.Should().BeTrue();
    }

    [Fact]
    public void HallImage_Should_Assign_IsActive()
    {
        var image = new HallImage();
        image.IsActive = true;
        image.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallImage_Should_Assign_All_Properties()
    {
        var hallImageId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var image = new HallImage
        {
            HallImageId = hallImageId,
            HallId = hallId,
            ImagePath = "/images/hall/main.jpg",
            ThumbnailUrl = "/images/hall/thumb.jpg",
            DisplayOrder = "1",
            IsCoverImage = true,
            IsActive = true
        };

        image.HallImageId.Should().Be(hallImageId);
        image.HallId.Should().Be(hallId);
        image.ImagePath.Should().Be("/images/hall/main.jpg");
        image.ThumbnailUrl.Should().Be("/images/hall/thumb.jpg");
        image.DisplayOrder.Should().Be("1");
        image.IsCoverImage.Should().BeTrue();
        image.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallImage_Should_Have_Default_Values()
    {
        var image = new HallImage();
        image.HallImageId.Should().Be(Guid.Empty);
        image.HallId.Should().Be(Guid.Empty);
        image.ImagePath.Should().BeEmpty();
        image.ThumbnailUrl.Should().BeEmpty();
        image.DisplayOrder.Should().BeEmpty();
        image.IsCoverImage.Should().BeFalse();
        image.IsActive.Should().BeFalse();
    }
}