using System.Net;
using FluentAssertions;

namespace BookMyHall.Api.Tests.Endpoints.System;

public sealed class RunnerEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetRunners_ShouldReturnOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/runners");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StartRunner_WithUnknownService_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/api/runners/does-not-exist-123/start", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DisableRunner_WithUnknownService_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/api/runners/does-not-exist-456/disable", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
