using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Masters;

public sealed class ServiceTests
{
    [Fact]
    public void Service_Should_Be_Inactive_By_Default()
    {
        var service = new Service();
        service.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Service_Should_Assign_ServiceId()
    {
        var service = new Service();
        var id = Guid.NewGuid();
        service.ServiceId = id;
        service.ServiceId.Should().Be(id);
    }

    [Fact]
    public void Service_Should_Assign_ServiceName()
    {
        var service = new Service();
        service.ServiceName = "Catering";
        service.ServiceName.Should().Be("Catering");
    }

    [Fact]
    public void Service_Should_Assign_ServiceIcon()
    {
        var service = new Service();
        service.ServiceIcon = "catering-icon.png";
        service.ServiceIcon.Should().Be("catering-icon.png");
    }

    [Fact]
    public void Service_Should_Assign_IsActive()
    {
        var service = new Service();
        service.IsActive = true;
        service.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Service_Should_Assign_All_Properties()
    {
        var serviceId = Guid.NewGuid();
        var service = new Service
        {
            ServiceId = serviceId,
            ServiceName = "Catering",
            ServiceIcon = "catering-icon.png",
            IsActive = true
        };

        service.ServiceId.Should().Be(serviceId);
        service.ServiceName.Should().Be("Catering");
        service.ServiceIcon.Should().Be("catering-icon.png");
        service.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Service_Should_Have_Default_Values()
    {
        var service = new Service();
        service.ServiceId.Should().Be(Guid.Empty);
        service.ServiceName.Should().BeEmpty();
        service.ServiceIcon.Should().BeEmpty();
        service.IsActive.Should().BeFalse();
    }
}