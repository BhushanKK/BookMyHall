using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class CityTests
{
    [Fact]
    public void City_Should_Be_Inactive_By_Default()
    {
        var city = new City();
        city.IsActive.Should().BeFalse();
    }

    [Fact]
    public void City_Should_Assign_CityId()
    {
        var city = new City();
        var id = Guid.NewGuid();
        city.CityId = id;
        city.CityId.Should().Be(id);
    }

    [Fact]
    public void City_Should_Assign_DistrictId()
    {
        var city = new City();
        var districtId = Guid.NewGuid();
        city.DistrictId = districtId;
        city.DistrictId.Should().Be(districtId);
    }

    [Fact]
    public void City_Should_Assign_CityName()
    {
        var city = new City();
        city.CityName = "Pune";
        city.CityName.Should().Be("Pune");
    }

    [Fact]
    public void City_Should_Assign_IsActive()
    {
        var city = new City();
        city.IsActive = true;
        city.IsActive.Should().BeTrue();
    }

    [Fact]
    public void City_Should_Assign_All_Properties()
    {
        var cityId = Guid.NewGuid();
        var districtId = Guid.NewGuid();
        var city = new City
        {
            CityId = cityId,
            DistrictId = districtId,
            CityName = "Pune",
            IsActive = true
        };

        city.CityId.Should().Be(cityId);
        city.DistrictId.Should().Be(districtId);
        city.CityName.Should().Be("Pune");
        city.IsActive.Should().BeTrue();
    }

    [Fact]
    public void City_Should_Have_Default_Values()
    {
        var city = new City();
        city.CityId.Should().Be(Guid.Empty);
        city.DistrictId.Should().Be(Guid.Empty);
        city.CityName.Should().BeEmpty();
        city.IsActive.Should().BeFalse();
    }
}