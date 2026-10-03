using System.Net;
using System.Net.Http.Headers;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using FluentAssertions;

namespace BookMyHall.Api.Tests.Endpoints.Venue;

public sealed class VendorImageEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    private readonly BookMyHallWebApplicationFactory _factory = factory;

    [Fact]
    public async Task UploadAndGalleryBindServiceCategoryAndSubcategory()
    {
        var vendorId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var subCategoryId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<CreateVendorImageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<Guid>.SuccessResponse(Guid.NewGuid(), statusCode: HttpStatusCode.Created));
        mediator.Setup(x => x.Send(It.IsAny<GetVendorImagesByVendorIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<PaginatedResult<VendorImageDto>>.SuccessResponse(new() { Items = [] }));
        using var app = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(mediator.Object);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = LogoTestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = LogoTestAuthenticationHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, LogoTestAuthenticationHandler>(LogoTestAuthenticationHandler.SchemeName, _ => { });
        }));
        using var client = app.CreateClient();
        using var content = CreateMultipartContent();
        using var uploaded = await client.PostAsync($"/api/vendors/{vendorId}/images?vendorServiceId={serviceId}&displayOrder=1&isCoverImage=true", content);
        uploaded.StatusCode.Should().Be(HttpStatusCode.Created);
        mediator.Verify(x => x.Send(It.Is<CreateVendorImageCommand>(command => command.VendorId == vendorId &&
            command.VendorServiceId == serviceId && command.IsCoverImage), It.IsAny<CancellationToken>()), Times.Once);
        using var gallery = await client.GetAsync($"/api/vendors/{vendorId}/images?pageNumber=1&pageSize=12&sortDescending=false&vendorServiceId={serviceId}&vendorCategoryId={categoryId}&vendorSubCategoryId={subCategoryId}");
        gallery.StatusCode.Should().Be(HttpStatusCode.OK);
        mediator.Verify(x => x.Send(It.Is<GetVendorImagesByVendorIdQuery>(query => query.VendorId == vendorId &&
            query.VendorServiceId == serviceId && query.VendorCategoryId == categoryId && query.VendorSubCategoryId == subCategoryId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

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
    public async Task GetVendorBusinessesAutoComplete_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/vendors/vendor/{Guid.NewGuid()}/businesses/autocomplete");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVendorBusinessesAutoComplete_WithInvalidVendorId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/vendors/vendor/not-a-guid/businesses/autocomplete");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
