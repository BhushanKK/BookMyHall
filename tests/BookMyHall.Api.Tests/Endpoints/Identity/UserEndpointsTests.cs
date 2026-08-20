using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class UserEndpointsTests(BookMyHallWebApplicationFactory factory): IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    #region Create User

    [Fact]
    public async Task CreateUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            firstName = "Test",
            middleName = "User",
            lastName = "Account",
            mobileNumber = "9876543210",
            emailAddress = "test@example.com",
            password = "Test@123",
            dateOfBirth = "1998-02-14",
            gender = 1,
            roleId = Guid.NewGuid()
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/users",
            request);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get Users

    [Fact]
    public async Task GetUsers_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/users");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get User By Id

    [Fact]
    public async Task GetUserById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/users/{userId}");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/users/not-a-guid");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update User

    [Fact]
    public async Task UpdateUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        var request = new
        {
            firstName = "Updated",
            middleName = "Test",
            lastName = "User",
            mobileNumber = "9876543210",
            dateOfBirth = "1998-02-14",
            gender = 1,
            emailAddress = "updated@example.com",
            roleId = Guid.NewGuid()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/users/{userId}",
            request);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            firstName = "Updated",
            middleName = "Test",
            lastName = "User",
            mobileNumber = "9876543210",
            dateOfBirth = "1998-02-14",
            gender = 1,
            emailAddress = "updated@example.com",
            roleId = Guid.NewGuid()
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/users/not-a-guid",
            request);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update Profile Image

    [Fact]
    public async Task UpdateUserProfileImage_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        using var content = CreateProfileImageContent();

        // Act
        var response = await client.PutAsync(
            $"/api/users/{userId}/profile-image",
            content);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUserProfileImage_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var content = CreateProfileImageContent();

        // Act
        var response = await client.PutAsync(
            "/api/users/not-a-guid/profile-image",
            content);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserProfileImage_WithoutImage_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        using var content = new MultipartFormDataContent();

        // Act
        var response = await client.PutAsync(
            $"/api/users/{userId}/profile-image",
            content);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Delete User

    [Fact]
    public async Task DeleteUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/users/{userId}");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/users/not-a-guid");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helpers

    private static MultipartFormDataContent CreateProfileImageContent()
    {
        var content = new MultipartFormDataContent();

        var imageBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0,
            0x00, 0x10, 0x4A, 0x46,
            0x49, 0x46
        };

        var imageContent = new ByteArrayContent(imageBytes);

        imageContent.Headers.ContentType =
            new MediaTypeHeaderValue("image/jpeg");

        content.Add(
            imageContent,
            "profileImage",
            "profile.jpg");

        return content;
    }

    #endregion
}