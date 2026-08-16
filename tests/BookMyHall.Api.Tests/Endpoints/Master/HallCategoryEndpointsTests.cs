using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookMyHall.Api.Tests;

public sealed class HallCategoryEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetHallCategories_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/hall-categories");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallCategoryById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var hallCategoryId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/hall-categories/{hallCategoryId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHallCategoryById_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.GetAsync(
            "/api/hall-categories/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHallCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            hallCategoryName = "Premium Hall"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/hall-categories",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var hallCategoryId = Guid.NewGuid();

        var request = new
        {
            hallCategoryName = "Updated Hall Category"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/hall-categories/{hallCategoryId}",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateHallCategory_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        var request = new
        {
            hallCategoryName = "Updated Hall Category"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/hall-categories/not-a-guid",
            request);

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHallCategory_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        var hallCategoryId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync(
            $"/api/hall-categories/{hallCategoryId}");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteHallCategory_WithInvalidRouteId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        var response = await client.DeleteAsync(
            "/api/hall-categories/not-a-guid");

        // Assert
        await AssertStatusCodeAsync(
            response,
            HttpStatusCode.NotFound);
    }

    // =========================================================
    // Helpers
    // =========================================================

    private HttpClient CreateClient()
    {
        return _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    private static async Task AssertStatusCodeAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatusCode)
    {
        var responseBody =
            await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            expectedStatusCode,
            $"Actual status code: {(int)response.StatusCode} {response.StatusCode}. " +
            $"Response body: {responseBody}");
    }
}