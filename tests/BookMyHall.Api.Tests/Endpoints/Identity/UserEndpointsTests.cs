using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class UserEndpointsTests(
    BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    #region Create User

    [Fact]
    public async Task CreateUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
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

        var response = await client.PostAsJsonAsync(
            "/api/users",
            request);

        response.StatusCode.Should()
            .NotBe(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get Users

    [Fact]
    public async Task GetUsers_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/users");

        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Get User By Id

    [Fact]
    public async Task GetUserById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        var response = await client.GetAsync(
            $"/api/users/{userId}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/users/not-a-guid");

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update User

    [Fact]
    public async Task UpdateUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        using var content = CreateUpdateUserContent();

        var response = await client.PutAsync(
            $"/api/users/{userId}",
            content);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidRouteId_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();

        using var content = CreateUpdateUserContent();

        var response = await client.PutAsync(
            "/api/users/not-a-guid",
            content);

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_WithImage_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        using var content = CreateUpdateUserContent(
            includeImage: true);

        var response = await client.PutAsync(
            $"/api/users/{userId}",
            content);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Delete User

    [Fact]
    public async Task DeleteUser_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();

        var response = await client.DeleteAsync(
            $"/api/users/{userId}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidRouteId_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync(
            "/api/users/not-a-guid");

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helpers

    private static MultipartFormDataContent CreateUpdateUserContent(
        bool includeImage = false)
    {
        var content = new MultipartFormDataContent();

        content.Add(
            new StringContent("Updated"),
            "firstName");

        content.Add(
            new StringContent("Test"),
            "middleName");

        content.Add(
            new StringContent("User"),
            "lastName");

        content.Add(
            new StringContent("9876543210"),
            "mobileNumber");

        content.Add(
            new StringContent("1998-02-14"),
            "dateOfBirth");

        content.Add(
            new StringContent("1"),
            "gender");

        content.Add(
            new StringContent("updated@example.com"),
            "emailAddress");

        content.Add(
            new StringContent(Guid.NewGuid().ToString()),
            "roleId");

        if (includeImage)
        {
            var imageContent = new ByteArrayContent(
                CreateJpegBytes());

            imageContent.Headers.ContentType =
                new MediaTypeHeaderValue("image/jpeg");

            content.Add(
                imageContent,
                "image",
                "profile.jpg");
        }

        return content;
    }

    private static byte[] CreateJpegBytes()
    {
        return
        [
            0xFF, 0xD8, 0xFF, 0xE0,
            0x00, 0x10, 0x4A, 0x46,
            0x49, 0x46
        ];
    }

    #endregion
}