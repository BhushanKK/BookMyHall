using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BookMyHall.Api.Tests.Endpoints.Venue;

public sealed class VendorLogoEndpointsTests(BookMyHallWebApplicationFactory factory)
    : IClassFixture<BookMyHallWebApplicationFactory>
{
    [Theory]
    [InlineData("GET", "")]
    [InlineData("GET", "/content")]
    [InlineData("PUT", "")]
    [InlineData("DELETE", "")]
    public async Task LogoEndpoints_WithoutAuthenticationReturnUnauthorized(string method, string suffix)
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod(method), $"/api/vendors/{Guid.NewGuid()}/logo{suffix}");
        if (method == "PUT")
        {
            request.Content = LogoContent();
        }

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Upload_BindsMultipartLogoAndVendorId()
    {
        var vendorId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<UploadVendorLogoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<VendorLogoDto>.SuccessResponse(new(vendorId, "https://example.test/logo.png"), "Uploaded"));
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
        using var content = LogoContent();

        using var response = await client.PutAsync($"/api/vendors/{vendorId}/logo", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        mediator.Verify(x => x.Send(It.Is<UploadVendorLogoCommand>(command =>
            command.VendorId == vendorId && command.FileName == "logo.png" &&
            command.ContentType == "image/png" && command.FileSize == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static MultipartFormDataContent LogoContent()
    {
        var content = new MultipartFormDataContent();
        var file = new ByteArrayContent([1, 2, 3]);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(file, "logo", "logo.png");
        return content;
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    public async Task VendorCreateAndUpdate_BindFormFieldsAndLogoFile(string method)
    {
        var vendorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<CreateVendorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<VendorDto>.SuccessResponse(new VendorDto { VendorId = vendorId }, statusCode: HttpStatusCode.Created));
        mediator.Setup(x => x.Send(It.IsAny<UpdateVendorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<VendorDto>.SuccessResponse(new VendorDto { VendorId = vendorId }));
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
        using var content = LogoContent();
        content.Add(new StringContent("Vendor business"), "BusinessName");
        content.Add(new StringContent(userId.ToString()), "UserId");
        using var request = new HttpRequestMessage(new HttpMethod(method), method == "POST" ? "/api/vendors/" : $"/api/vendors/{vendorId}") { Content = content };

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(method == "POST" ? HttpStatusCode.Created : HttpStatusCode.OK);
        if (method == "POST")
        {
            mediator.Verify(x => x.Send(It.Is<CreateVendorCommand>(command =>
                command.BusinessName == "Vendor business" && command.UserId == userId &&
                command.Logo != null && command.Logo.FileName == "logo.png" && command.Logo.FileSize == 3),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        else
        {
            mediator.Verify(x => x.Send(It.Is<UpdateVendorCommand>(command =>
                command.VendorId == vendorId && command.BusinessName == "Vendor business" &&
                command.Logo != null && command.Logo.ContentType == "image/png" && command.Logo.FileSize == 3),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

public sealed class LogoTestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "VendorLogoTest";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Role, RoleConstants.Admin)], SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
