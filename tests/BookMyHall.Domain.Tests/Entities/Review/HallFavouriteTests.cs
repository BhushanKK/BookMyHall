using FluentAssertions;
using BookMyHall.Domain.Review;

namespace BookMyHall.Domain.Tests.Review;

public sealed class HallFavouriteTests
{
    [Fact]
    public void HallFavourite_Should_Assign_HallFavouriteId()
    {
        var hallFavourite = new HallFavourite();
        var id = Guid.NewGuid();
        hallFavourite.HallFavouriteId = id;
        hallFavourite.HallFavouriteId.Should().Be(id);
    }

    [Fact]
    public void HallFavourite_Should_Assign_CustomerId()
    {
        var hallFavourite = new HallFavourite();
        var customerId = Guid.NewGuid();
        hallFavourite.CustomerId = customerId;
        hallFavourite.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public void HallFavourite_Should_Assign_HallId()
    {
        var hallFavourite = new HallFavourite();
        var hallId = Guid.NewGuid();
        hallFavourite.HallId = hallId;
        hallFavourite.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallFavourite_Should_Assign_All_Properties()
    {
        var hallFavouriteId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var hallFavourite = new HallFavourite
        {
            HallFavouriteId = hallFavouriteId,
            CustomerId = customerId,
            HallId = hallId
        };

        hallFavourite.HallFavouriteId.Should().Be(hallFavouriteId);
        hallFavourite.CustomerId.Should().Be(customerId);
        hallFavourite.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallFavourite_Should_Have_Default_Values()
    {
        var hallFavourite = new HallFavourite();
        hallFavourite.HallFavouriteId.Should().Be(Guid.Empty);
        hallFavourite.CustomerId.Should().Be(Guid.Empty);
        hallFavourite.HallId.Should().Be(Guid.Empty);
    }
}