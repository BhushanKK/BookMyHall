using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class FacilityTests
{
    [Fact]
    public void Facility_Should_Be_Inactive_By_Default()
    {
        var facility = new Facility();
        facility.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Facility_Should_Assign_FacilityId()
    {
        var facility = new Facility();
        var id = Guid.NewGuid();
        facility.FacilityId = id;
        facility.FacilityId.Should().Be(id);
    }

    [Fact]
    public void Facility_Should_Assign_FacilityName()
    {
        var facility = new Facility();
        facility.FacilityName = "Parking";
        facility.FacilityName.Should().Be("Parking");
    }

    [Fact]
    public void Facility_Should_Assign_FacilityIcon()
    {
        var facility = new Facility();
        facility.FacilityIcon = "parking-icon.png";
        facility.FacilityIcon.Should().Be("parking-icon.png");
    }

    [Fact]
    public void Facility_Should_Assign_IsActive()
    {
        var facility = new Facility();
        facility.IsActive = true;
        facility.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Facility_Should_Assign_All_Properties()
    {
        var facilityId = Guid.NewGuid();
        var facility = new Facility
        {
            FacilityId = facilityId,
            FacilityName = "Parking",
            FacilityIcon = "parking-icon.png",
            IsActive = true
        };

        facility.FacilityId.Should().Be(facilityId);
        facility.FacilityName.Should().Be("Parking");
        facility.FacilityIcon.Should().Be("parking-icon.png");
        facility.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Facility_Should_Have_Default_Values()
    {
        var facility = new Facility();

        facility.FacilityId.Should().Be(Guid.Empty);
        facility.FacilityName.Should().BeEmpty();
        facility.FacilityIcon.Should().BeEmpty();
        facility.IsActive.Should().BeFalse();
    }
}