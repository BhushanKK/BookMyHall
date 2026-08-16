using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

namespace BookMyHall.Api.Tests;

public sealed class HallImageEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CreateHallImage_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallId = Guid.NewGuid();

        using var content = CreateMultipartContent();

        // Act
        var response = await client.PostAsync(
            $"/api/halls/{hallId}/images" +
            "?displayOrder=1&isCoverImage=true",
            content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateHallImage_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var content = CreateMultipartContent();

        // Act
        var response = await client.PostAsync(
            "/api/halls/not-a-guid/images" +
            "?displayOrder=1&isCoverImage=true",
            content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHallImageById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallImageId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/halls/images/{hallImageId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallImageById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls/images/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHallImagesByHallId_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/halls/{hallId}/images");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallImagesByHallId_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls/not-a-guid/images");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHallCoverImage_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/halls/{hallId}/cover-image");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallCoverImage_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls/not-a-guid/cover-image");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateHallImage_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallImageId = Guid.NewGuid();
        var request = new
        {
            isCoverImage = true,
            displayOrder = 1,
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/halls/images/{hallImageId}",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallImage_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            isCoverImage = true,
            displayOrder = 1,
            isActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/halls/images/not-a-guid",request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHallImage_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallImageId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/halls/images/{hallImageId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteHallImage_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/halls/images/not-a-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHallImageContent_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var hallImageId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/halls/images/{hallImageId}/content");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallImageContent_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/halls/images/not-a-guid/content");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static MultipartFormDataContent CreateMultipartContent()
    {
        var content = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(
        [
            0xFF,
            0xD8,
            0xFF,
            0xE0
        ]);

        imageContent.Headers.ContentType =new MediaTypeHeaderValue("image/jpeg");
        content.Add(imageContent,"image","test-image.jpg");
        return content;
    }
}