using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class AuthenticationEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task Login_WithoutAuthentication_ShouldNotReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            emailAddress = "test@example.com",
            password = "Password@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/login",request);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            refreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/logout", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            currentPassword = "OldPassword@123",
            newPassword = "NewPassword@123",
            confirmPassword = "NewPassword@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/change-password",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ForgotPassword_WithoutAuthentication_ShouldNotReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            emailAddress = "test@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/forgot-password", request);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_WithoutAuthentication_ShouldNotReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            emailAddress = "test@example.com",
            token = "invalid-token",
            newPassword = "NewPassword@123",
            confirmPassword = "NewPassword@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/reset-password",request);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task VerifyEmail_WithoutAuthentication_ShouldNotReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            emailAddress = "test@example.com",
            token = "invalid-token"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/verify-email",request);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResendVerificationEmail_WithoutAuthentication_ShouldNotReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            emailAddress = "test@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/authentication/resend-verification-email",request);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}