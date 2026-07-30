using FluentAssertions;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Domain.Tests.Venue;

public sealed class HallTests
{
    [Fact]
    public void Hall_Should_Be_Inactive_By_Default()
    {
        var hall = new Hall();
        hall.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Hall_Should_Assign_HallId()
    {
        var hall = new Hall();
        var id = Guid.NewGuid();
        hall.HallId = id;
        hall.HallId.Should().Be(id);
    }

    [Fact]
    public void Hall_Should_Assign_HallName()
    {
        var hall = new Hall();
        hall.HallName = "Royal Banquet Hall";
        hall.HallName.Should().Be("Royal Banquet Hall");
    }

    [Fact]
    public void Hall_Should_Assign_HallOwnerId()
    {
        var hall = new Hall();
        var ownerId = Guid.NewGuid();
        hall.HallOwnerId = ownerId;
        hall.HallOwnerId.Should().Be(ownerId);
    }

    [Fact]
    public void Hall_Should_Assign_HallCategoryId()
    {
        var hall = new Hall();
        var categoryId = Guid.NewGuid();
        hall.HallCategoryId = categoryId;
        hall.HallCategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Hall_Should_Assign_CancellationPolicyId()
    {
        var hall = new Hall();
        var policyId = Guid.NewGuid();
        hall.CancellationPolicyId = policyId;
        hall.CancellationPolicyId.Should().Be(policyId);
    }

    [Fact]
    public void Hall_Should_Assign_Description()
    {
        var hall = new Hall();
        hall.Description = "Luxury wedding venue.";
        hall.Description.Should().Be("Luxury wedding venue.");
    }

    [Fact]
    public void Hall_Should_Assign_AddressLine1()
    {
        var hall = new Hall();
        hall.AddressLine1 = "MG Road";
        hall.AddressLine1.Should().Be("MG Road");
    }

    [Fact]
    public void Hall_Should_Assign_AddressLine2()
    {
        var hall = new Hall();
        hall.AddressLine2 = "Near Bus Stand";
        hall.AddressLine2.Should().Be("Near Bus Stand");
    }

    [Fact]
    public void Hall_Should_Assign_AreaId()
    {
        var hall = new Hall();
        var areaId = Guid.NewGuid();
        hall.AreaId = areaId;
        hall.AreaId.Should().Be(areaId);
    }

    [Fact]
    public void Hall_Should_Assign_Pincode()
    {
        var hall = new Hall();
        hall.Pincode = "411001";
        hall.Pincode.Should().Be("411001");
    }

    [Fact]
    public void Hall_Should_Assign_Latitude()
    {
        var hall = new Hall();
        hall.Latitude = 18.5204m;
        hall.Latitude.Should().Be(18.5204m);
    }

    [Fact]
    public void Hall_Should_Assign_Longitude()
    {
        var hall = new Hall();
        hall.Longitude = 73.8567m;
        hall.Longitude.Should().Be(73.8567m);
    }

    [Fact]
    public void Hall_Should_Assign_ContactPersonName()
    {
        var hall = new Hall();
        hall.ContactPersonName = "Aniket Yadav";
        hall.ContactPersonName.Should().Be("Aniket Yadav");
    }

    [Fact]
    public void Hall_Should_Assign_MobileNumber()
    {
        var hall = new Hall();
        hall.MobileNumber = "9876543210";
        hall.MobileNumber.Should().Be("9876543210");
    }

    [Fact]
    public void Hall_Should_Assign_EmailAddress()
    {
        var hall = new Hall();
        hall.EmailAddress = "hall@example.com";
        hall.EmailAddress.Should().Be("hall@example.com");
    }

    [Fact]
    public void Hall_Should_Assign_AlternateMobileNumber()
    {
        var hall = new Hall();
        hall.AlternateMobileNumber = "9876500000";
        hall.AlternateMobileNumber.Should().Be("9876500000");
    }

    [Fact]
    public void Hall_Should_Assign_Website()
    {
        var hall = new Hall();
        hall.Website = "https://royalhall.com";
        hall.Website.Should().Be("https://royalhall.com");
    }

    [Fact]
    public void Hall_Should_Assign_MinimumCapacity()
    {
        var hall = new Hall();
        hall.MinimumCapacity = "100";
        hall.MinimumCapacity.Should().Be("100");
    }

    [Fact]
    public void Hall_Should_Assign_MaximumCapacity()
    {
        var hall = new Hall();
        hall.MaximumCapacity = "500";
        hall.MaximumCapacity.Should().Be("500");
    }

    [Fact]
    public void Hall_Should_Assign_CheckInTime()
    {
        var hall = new Hall();
        var time = new TimeSpan(9, 0, 0);
        hall.CheckInTime = time;
        hall.CheckInTime.Should().Be(time);
    }

    [Fact]
    public void Hall_Should_Assign_CheckOutTime()
    {
        var hall = new Hall();
        var time = new TimeSpan(22, 0, 0);
        hall.CheckOutTime = time;
        hall.CheckOutTime.Should().Be(time);
    }

    [Fact]
    public void Hall_Should_Assign_GoogleMapLocationUrl()
    {
        var hall = new Hall();
        hall.GoogleMapLocationUrl = "https://maps.google.com/test";
        hall.GoogleMapLocationUrl.Should().Be("https://maps.google.com/test");
    }

    [Fact]
    public void Hall_Should_Assign_ApprovalStatus()
    {
        var hall = new Hall();
        hall.ApprovalStatus = "Approved";
        hall.ApprovalStatus.Should().Be("Approved");
    }

    [Fact]
    public void Hall_Should_Assign_VerificationStatus()
    {
        var hall = new Hall();
        hall.VerificationStatus = "Verified";
        hall.VerificationStatus.Should().Be("Verified");
    }

    [Fact]
    public void Hall_Should_Assign_IsActive()
    {
        var hall = new Hall();
        hall.IsActive = true;
        hall.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Hall_Should_Assign_All_Properties()
    {
        var hallId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var policyId = Guid.NewGuid();
        var areaId = Guid.NewGuid();
        var hall = new Hall
        {
            HallId = hallId,
            HallName = "Royal Banquet Hall",
            HallOwnerId = ownerId,
            HallCategoryId = categoryId,
            CancellationPolicyId = policyId,
            Description = "Luxury wedding venue.",
            AddressLine1 = "MG Road",
            AddressLine2 = "Near Bus Stand",
            AreaId = areaId,
            Pincode = "411001",
            Latitude = 18.5204m,
            Longitude = 73.8567m,
            ContactPersonName = "Aniket Yadav",
            MobileNumber = "9876543210",
            EmailAddress = "hall@example.com",
            AlternateMobileNumber = "9876500000",
            Website = "https://royalhall.com",
            MinimumCapacity = "100",
            MaximumCapacity = "500",
            CheckInTime = new TimeSpan(9, 0, 0),
            CheckOutTime = new TimeSpan(22, 0, 0),
            GoogleMapLocationUrl = "https://maps.google.com/test",
            ApprovalStatus = "Approved",
            VerificationStatus = "Verified",
            IsActive = true
        };

        hall.HallId.Should().Be(hallId);
        hall.HallName.Should().Be("Royal Banquet Hall");
        hall.HallOwnerId.Should().Be(ownerId);
        hall.HallCategoryId.Should().Be(categoryId);
        hall.CancellationPolicyId.Should().Be(policyId);
        hall.Description.Should().Be("Luxury wedding venue.");
        hall.AddressLine1.Should().Be("MG Road");
        hall.AddressLine2.Should().Be("Near Bus Stand");
        hall.AreaId.Should().Be(areaId);
        hall.Pincode.Should().Be("411001");
        hall.Latitude.Should().Be(18.5204m);
        hall.Longitude.Should().Be(73.8567m);
        hall.ContactPersonName.Should().Be("Aniket Yadav");
        hall.MobileNumber.Should().Be("9876543210");
        hall.EmailAddress.Should().Be("hall@example.com");
        hall.AlternateMobileNumber.Should().Be("9876500000");
        hall.Website.Should().Be("https://royalhall.com");
        hall.MinimumCapacity.Should().Be("100");
        hall.MaximumCapacity.Should().Be("500");
        hall.CheckInTime.Should().Be(new TimeSpan(9, 0, 0));
        hall.CheckOutTime.Should().Be(new TimeSpan(22, 0, 0));
        hall.GoogleMapLocationUrl.Should().Be("https://maps.google.com/test");
        hall.ApprovalStatus.Should().Be("Approved");
        hall.VerificationStatus.Should().Be("Verified");
        hall.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Hall_Should_Have_Default_Values()
    {
        var hall = new Hall();

        hall.HallId.Should().Be(Guid.Empty);
        hall.HallName.Should().BeEmpty();
        hall.HallOwnerId.Should().Be(Guid.Empty);
        hall.HallCategoryId.Should().Be(Guid.Empty);
        hall.CancellationPolicyId.Should().Be(Guid.Empty);
        hall.Description.Should().BeEmpty();
        hall.AddressLine1.Should().BeEmpty();
        hall.AddressLine2.Should().BeEmpty();
        hall.AreaId.Should().Be(Guid.Empty);
        hall.Pincode.Should().BeEmpty();
        hall.Latitude.Should().Be(0);
        hall.Longitude.Should().Be(0);
        hall.ContactPersonName.Should().BeEmpty();
        hall.MobileNumber.Should().BeEmpty();
        hall.EmailAddress.Should().BeEmpty();
        hall.AlternateMobileNumber.Should().BeEmpty();
        hall.Website.Should().BeEmpty();
        hall.MinimumCapacity.Should().BeEmpty();
        hall.MaximumCapacity.Should().BeEmpty();
        hall.CheckInTime.Should().Be(TimeSpan.Zero);
        hall.CheckOutTime.Should().Be(TimeSpan.Zero);
        hall.GoogleMapLocationUrl.Should().BeEmpty();
        hall.ApprovalStatus.Should().BeEmpty();
        hall.VerificationStatus.Should().BeEmpty();
        hall.IsActive.Should().BeFalse();
    }
}