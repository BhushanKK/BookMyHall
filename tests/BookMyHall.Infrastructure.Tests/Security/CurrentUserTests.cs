using Moq;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using BookMyHall.Infrastructure.Security;

namespace BookMyHall.Infrastructure.Tests.Security;

public sealed class CurrentUserTests
{
    private static CurrentUser CreateCurrentUser(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext
        {
            User = principal
        };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);
        return new CurrentUser(accessor.Object);
    }

    [Fact]
    public void IsAuthenticated_Should_Return_True_When_User_Is_Authenticated()
    {
        var currentUser = CreateCurrentUser();
        currentUser.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void UserId_Should_Return_UserId_When_Claim_Exists()
    {
        var userId = Guid.NewGuid();
        var currentUser = CreateCurrentUser(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        currentUser.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserId_Should_Return_Null_When_Claim_Is_Missing()
    {
        var currentUser = CreateCurrentUser();
        currentUser.UserId.Should().BeNull();
    }

    [Fact]
    public void UserId_Should_Return_Null_When_Claim_Is_Invalid_Guid()
    {
        var currentUser = CreateCurrentUser(new Claim(ClaimTypes.NameIdentifier, "ABC123"));
        currentUser.UserId.Should().BeNull();
    }

    [Fact]
    public void FullName_Should_Return_Name()
    {
        var currentUser = CreateCurrentUser(new Claim(ClaimTypes.Name, "John Doe"));
        currentUser.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void FullName_Should_Return_Null_When_Missing()
    {
        var currentUser = CreateCurrentUser();
        currentUser.FullName.Should().BeNull();
    }

    [Fact]
    public void MobileNumber_Should_Return_Mobile()
    {
        var currentUser = CreateCurrentUser(new Claim(ClaimTypes.MobilePhone, "9876543210"));
        currentUser.MobileNumber.Should().Be("9876543210");
    }

    [Fact]
    public void MobileNumber_Should_Return_Null_When_Missing()
    {
        var currentUser = CreateCurrentUser();
        currentUser.MobileNumber.Should().BeNull();
    }

    [Fact]
    public void EmailAddress_Should_Return_Email()
    {
        var currentUser = CreateCurrentUser(new Claim(ClaimTypes.Email, "john@test.com"));
        currentUser.EmailAddress.Should().Be("john@test.com");
    }

    [Fact]
    public void EmailAddress_Should_Return_Null_When_Missing()
    {
        var currentUser = CreateCurrentUser();
        currentUser.EmailAddress.Should().BeNull();
    }

    [Fact]
    public void Roles_Should_Return_All_Roles()
    {
        var currentUser = CreateCurrentUser(
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "HallOwner"));
        var roles = currentUser.Roles;
        roles.Should().HaveCount(2);
        roles.Should().Contain("Admin");
        roles.Should().Contain("HallOwner");
    }

    [Fact]
    public void Roles_Should_Return_Empty_When_No_Roles_Exist()
    {
        var currentUser = CreateCurrentUser();
        currentUser.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Should_Return_Default_Values_When_HttpContext_Is_Null()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var currentUser = new CurrentUser(accessor.Object);
        currentUser.IsAuthenticated.Should().BeFalse();
        currentUser.UserId.Should().BeNull();
        currentUser.FullName.Should().BeNull();
        currentUser.MobileNumber.Should().BeNull();
        currentUser.EmailAddress.Should().BeNull();
        currentUser.Roles.Should().BeEmpty();
    }
}