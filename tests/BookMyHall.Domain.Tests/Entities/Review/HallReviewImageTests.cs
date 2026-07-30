using FluentAssertions;
using BookMyHall.Domain.Review;

namespace BookMyHall.Domain.Tests.Review;

public sealed class HallReviewImageTests
{
    [Fact]
    public void HallReviewImage_Should_Assign_HallReviewImageId()
    {
        var hallReviewImage = new HallReviewImage();
        var id = Guid.NewGuid();
        hallReviewImage.HallReviewImageId = id;
        hallReviewImage.HallReviewImageId.Should().Be(id);
    }

    [Fact]
    public void HallReviewImage_Should_Assign_HallReviewId()
    {
        var hallReviewImage = new HallReviewImage();
        var reviewId = Guid.NewGuid();
        hallReviewImage.HallReviewId = reviewId;
        hallReviewImage.HallReviewId.Should().Be(reviewId);
    }

    [Fact]
    public void HallReviewImage_Should_Assign_ImageUrl()
    {
        var hallReviewImage = new HallReviewImage();
        hallReviewImage.ImageUrl = "/images/reviews/review1.jpg";
        hallReviewImage.ImageUrl.Should().Be("/images/reviews/review1.jpg");
    }

    [Fact]
    public void HallReviewImage_Should_Assign_DisplayOrder()
    {
        var hallReviewImage = new HallReviewImage();
        hallReviewImage.DisplayOrder = 1;
        hallReviewImage.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public void HallReviewImage_Should_Assign_All_Properties()
    {
        var imageId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var hallReviewImage = new HallReviewImage
        {
            HallReviewImageId = imageId,
            HallReviewId = reviewId,
            ImageUrl = "/images/reviews/review1.jpg",
            DisplayOrder = 1
        };

        hallReviewImage.HallReviewImageId.Should().Be(imageId);
        hallReviewImage.HallReviewId.Should().Be(reviewId);
        hallReviewImage.ImageUrl.Should().Be("/images/reviews/review1.jpg");
        hallReviewImage.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public void HallReviewImage_Should_Have_Default_Values()
    {
        var hallReviewImage = new HallReviewImage();
        hallReviewImage.HallReviewImageId.Should().Be(Guid.Empty);
        hallReviewImage.HallReviewId.Should().Be(Guid.Empty);
        hallReviewImage.ImageUrl.Should().BeEmpty();
        hallReviewImage.DisplayOrder.Should().Be(0);
    }
}