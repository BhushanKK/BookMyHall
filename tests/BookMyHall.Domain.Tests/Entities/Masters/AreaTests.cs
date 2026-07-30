using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class AreaTests
{
    [Fact]
    public void Area_Should_Be_Inactive_By_Default()
    {
        var area = new Area();
        area.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Area_Should_Assign_AreaId()
    {
        var area = new Area();
        var id = Guid.NewGuid();
        area.AreaId = id;
        area.AreaId.Should().Be(id);
    }

    [Fact]
    public void Area_Should_Assign_AreaName()
    {
        var area = new Area();
        area.AreaName = "Shivaji Nagar";
        area.AreaName.Should().Be("Shivaji Nagar");
    }

    [Fact]
    public void Area_Should_Assign_Pincode()
    {
        var area = new Area();
        area.Pincode = "411005";
        area.Pincode.Should().Be("411005");
    }

    [Fact]
    public void Area_Should_Assign_CityId()
    {
        var area = new Area();
        var cityId = Guid.NewGuid();
        area.CityId = cityId;
        area.CityId.Should().Be(cityId);
    }

    [Fact]
    public void Area_Should_Assign_IsActive()
    {
        var area = new Area();
        area.IsActive = true;
        area.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Area_Should_Assign_All_Properties()
    {
        var areaId = Guid.NewGuid();
        var cityId = Guid.NewGuid();
        var area = new Area
        {
            AreaId = areaId,
            AreaName = "Shivaji Nagar",
            Pincode = "411005",
            CityId = cityId,
            IsActive = true
        };

        area.AreaId.Should().Be(areaId);
        area.AreaName.Should().Be("Shivaji Nagar");
        area.Pincode.Should().Be("411005");
        area.CityId.Should().Be(cityId);
        area.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Area_Should_Have_Default_Values()
    {
        var area = new Area();
        area.AreaId.Should().Be(Guid.Empty);
        area.AreaName.Should().BeEmpty();
        area.Pincode.Should().BeEmpty();
        area.CityId.Should().Be(Guid.Empty);
        area.IsActive.Should().BeFalse();
    }
}