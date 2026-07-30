using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class AmenityTests
{
    [Fact]
    public void Amenity_Should_Be_Inactive_By_Default()
    {
        var amenity = new Amenity();
        amenity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Amenity_Should_Assign_AmenityId()
    {
        var amenity = new Amenity();
        var id = Guid.NewGuid();
        amenity.AmenityId = id;
        amenity.AmenityId.Should().Be(id);
    }

    [Fact]
    public void Amenity_Should_Assign_AmenityName()
    {
        var amenity = new Amenity();
        amenity.AmenityName = "Air Conditioning";
        amenity.AmenityName.Should().Be("Air Conditioning");
    }

    [Fact]
    public void Amenity_Should_Assign_AmenityIcon()
    {
        var amenity = new Amenity();
        amenity.AmenityIcon = "ac-icon.png";
        amenity.AmenityIcon.Should().Be("ac-icon.png");
    }

    [Fact]
    public void Amenity_Should_Assign_IsActive()
    {
        var amenity = new Amenity();
        amenity.IsActive = true;
        amenity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Amenity_Should_Assign_All_Properties()
    {
        var amenityId = Guid.NewGuid();
        var amenity = new Amenity
        {
            AmenityId = amenityId,
            AmenityName = "Air Conditioning",
            AmenityIcon = "ac-icon.png",
            IsActive = true
        };

        amenity.AmenityId.Should().Be(amenityId);
        amenity.AmenityName.Should().Be("Air Conditioning");
        amenity.AmenityIcon.Should().Be("ac-icon.png");
        amenity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Amenity_Should_Have_Default_Values()
    {
        var amenity = new Amenity();
        amenity.AmenityId.Should().Be(Guid.Empty);
        amenity.AmenityName.Should().BeEmpty();
        amenity.AmenityIcon.Should().BeEmpty();
        amenity.IsActive.Should().BeFalse();
    }
}