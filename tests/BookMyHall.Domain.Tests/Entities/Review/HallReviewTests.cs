using FluentAssertions;
using BookMyHall.Domain.Review;

namespace BookMyHall.Domain.Tests.Review;

public sealed class HallReviewTests
{
    [Fact]
    public void HallReview_Should_Be_Not_Approved_By_Default()
    {
        var hallReview = new HallReview();
        hallReview.IsApproved.Should().BeFalse();
    }

    [Fact]
    public void HallReview_Should_Assign_HallReviewId()
    {
        var hallReview = new HallReview();
        var id = Guid.NewGuid();
        hallReview.HallReviewId = id;
        hallReview.HallReviewId.Should().Be(id);
    }

    [Fact]
    public void HallReview_Should_Assign_HallId()
    {
        var hallReview = new HallReview();
        var hallId = Guid.NewGuid();
        hallReview.HallId = hallId;
        hallReview.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallReview_Should_Assign_BookingId()
    {
        var hallReview = new HallReview();
        var bookingId = Guid.NewGuid();
        hallReview.BookingId = bookingId;
        hallReview.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void HallReview_Should_Assign_CustomerId()
    {
        var hallReview = new HallReview();
        var customerId = Guid.NewGuid();
        hallReview.CustomerId = customerId;
        hallReview.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public void HallReview_Should_Assign_Rating()
    {
        var hallReview = new HallReview();
        hallReview.Rating = 5;
        hallReview.Rating.Should().Be(5);
    }

    [Fact]
    public void HallReview_Should_Assign_ReviewTitle()
    {
        var hallReview = new HallReview();
        hallReview.ReviewTitle = "Excellent Service";
        hallReview.ReviewTitle.Should().Be("Excellent Service");
    }

    [Fact]
    public void HallReview_Should_Assign_Review()
    {
        var hallReview = new HallReview();
        hallReview.Review = "The hall was clean and well managed.";
        hallReview.Review.Should().Be("The hall was clean and well managed.");
    }

    [Fact]
    public void HallReview_Should_Assign_IsApproved()
    {
        var hallReview = new HallReview();
        hallReview.IsApproved = true;
        hallReview.IsApproved.Should().BeTrue();
    }

    [Fact]
    public void HallReview_Should_Assign_All_Properties()
    {
        var hallReviewId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var hallReview = new HallReview
        {
            HallReviewId = hallReviewId,
            HallId = hallId,
            BookingId = bookingId,
            CustomerId = customerId,
            Rating = 5,
            ReviewTitle = "Excellent Service",
            Review = "The hall was clean and well managed.",
            IsApproved = true
        };

        hallReview.HallReviewId.Should().Be(hallReviewId);
        hallReview.HallId.Should().Be(hallId);
        hallReview.BookingId.Should().Be(bookingId);
        hallReview.CustomerId.Should().Be(customerId);
        hallReview.Rating.Should().Be(5);
        hallReview.ReviewTitle.Should().Be("Excellent Service");
        hallReview.Review.Should().Be("The hall was clean and well managed.");
        hallReview.IsApproved.Should().BeTrue();
    }

    [Fact]
    public void HallReview_Should_Have_Default_Values()
    {
        var hallReview = new HallReview();
        hallReview.HallReviewId.Should().Be(Guid.Empty);
        hallReview.HallId.Should().Be(Guid.Empty);
        hallReview.BookingId.Should().Be(Guid.Empty);
        hallReview.CustomerId.Should().Be(Guid.Empty);
        hallReview.Rating.Should().Be(0);
        hallReview.ReviewTitle.Should().BeEmpty();
        hallReview.Review.Should().BeEmpty();
        hallReview.IsApproved.Should().BeFalse();
    }
}