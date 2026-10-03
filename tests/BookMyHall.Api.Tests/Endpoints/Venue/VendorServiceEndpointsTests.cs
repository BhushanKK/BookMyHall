using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BookMyHall.Api.Tests.Endpoints.Venue;

public sealed class VendorServiceEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("GET")]
    [InlineData("LIST")]
    public async Task ServiceEndpointsReturnIdentifiersAndUseCorrectFilters(string operation)
    {
        var serviceId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var subCategoryId = Guid.NewGuid();
        var service = new VendorServiceDto
        {
            VendorServiceId = serviceId, VendorId = vendorId, VendorCategoryId = categoryId,
            VendorSubCategoryId = subCategoryId, ServiceName = "Bridal Mehandi", BusinessName = "Wedding Services"
        };
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<CreateVendorServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<VendorServiceDto>.SuccessResponse(service, statusCode: HttpStatusCode.Created));
        mediator.Setup(x => x.Send(It.IsAny<UpdateVendorServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<VendorServiceDto>.SuccessResponse(service));
        mediator.Setup(x => x.Send(It.IsAny<GetVendorServiceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<VendorServiceDto>.SuccessResponse(service));
        mediator.Setup(x => x.Send(It.IsAny<GetVendorServicesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<PaginatedResponse<VendorServiceDto>>.SuccessResponse(new() { Items = [service], TotalRecords = 1 }));
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
        using var response = operation switch
        {
            "POST" => await client.PostAsJsonAsync("/api/vendor-services/", service),
            "PUT" => await client.PutAsJsonAsync($"/api/vendor-services/{serviceId}", service),
            "GET" => await client.GetAsync($"/api/vendor-services/{serviceId}"),
            _ => await client.GetAsync($"/api/vendor-services/?pageNumber=1&pageSize=10&sortDescending=false&vendorId={vendorId}&vendorCategoryId={categoryId}&vendorSubCategoryId={subCategoryId}")
        };
        response.StatusCode.Should().Be(operation == "POST" ? HttpStatusCode.Created : HttpStatusCode.OK);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        if (operation == "LIST")
        {
            data.GetProperty("totalRecords").GetInt32().Should().Be(1);
            data = data.GetProperty("items")[0];
            mediator.Verify(x => x.Send(It.Is<GetVendorServicesQuery>(query => query.VendorId == vendorId &&
                query.VendorCategoryId == categoryId && query.VendorSubCategoryId == subCategoryId), It.IsAny<CancellationToken>()), Times.Once);
        }
        if (operation == "PUT") mediator.Verify(x => x.Send(It.Is<UpdateVendorServiceCommand>(command =>
            command.VendorServiceId == serviceId), It.IsAny<CancellationToken>()), Times.Once);
        data.GetProperty("vendorServiceId").GetGuid().Should().Be(serviceId);
        data.GetProperty("businessName").GetString().Should().Be("Wedding Services");
        data.TryGetProperty("vendor", out _).Should().BeFalse();
    }
}
