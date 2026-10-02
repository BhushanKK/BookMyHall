using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace BookMyHall.Api.Tests.Endpoints.Venue;

public sealed class VendorImageEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CreateVendorImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        using var content = CreateMultipartContent();

        var response = await client.PostAsync(
            $"/api/vendors/{Guid.NewGuid()}/images?displayOrder=1&isCoverImage=true",
            content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateVendorImage_WithInvalidRouteId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        using var content = CreateMultipartContent();

        var response = await client.PostAsync(
            "/api/vendors/not-a-guid/images?displayOrder=1&isCoverImage=true",
            content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVendorImageById_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/vendors/images/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVendorImages_WithInvalidRouteId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/vendors/not-a-guid/images");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVendorCoverImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/vendors/{Guid.NewGuid()}/cover-image");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteVendorImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/api/vendors/images/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVendorImageContent_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/vendors/images/{Guid.NewGuid()}/content");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static MultipartFormDataContent CreateMultipartContent()
    {
        var content = new MultipartFormDataContent();
        var image = new ByteArrayContent([0xFF, 0xD8, 0xFF, 0xE0]);
        image.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(image, "image", "test-image.jpg");
        return content;
    }
}