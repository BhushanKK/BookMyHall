using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class DistrictTests
{
    [Fact]
    public void District_Should_Be_Inactive_By_Default()
    {
        var district = new District();
        district.IsActive.Should().BeFalse();
    }

    [Fact]
    public void District_Should_Assign_DistrictId()
    {
        var district = new District();
        var id = Guid.NewGuid();
        district.DistrictId = id;
        district.DistrictId.Should().Be(id);
    }

    [Fact]
    public void District_Should_Assign_DistrictName()
    {
        var district = new District();
        district.DistrictName = "Nashik";
        district.DistrictName.Should().Be("Nashik");
    }

    [Fact]
    public void District_Should_Assign_StateId()
    {
        var district = new District();
        var stateId = Guid.NewGuid();
        district.StateId = stateId;
        district.StateId.Should().Be(stateId);
    }

    [Fact]
    public void District_Should_Assign_IsActive()
    {
        var district = new District();
        district.IsActive = true;
        district.IsActive.Should().BeTrue();
    }

    [Fact]
    public void District_Should_Assign_All_Properties()
    {
        var districtId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var district = new District
        {
            DistrictId = districtId,
            DistrictName = "Nashik",
            StateId = stateId,
            IsActive = true
        };

        district.DistrictId.Should().Be(districtId);
        district.DistrictName.Should().Be("Nashik");
        district.StateId.Should().Be(stateId);
        district.IsActive.Should().BeTrue();
    }

    [Fact]
    public void District_Should_Have_Default_Values()
    {
        var district = new District();
        district.DistrictId.Should().Be(Guid.Empty);
        district.DistrictName.Should().BeEmpty();
        district.StateId.Should().Be(Guid.Empty);
        district.IsActive.Should().BeFalse();
    }
}