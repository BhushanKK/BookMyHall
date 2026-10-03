using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Constants;
using MediatR;

namespace BookMyHall.Api.Endpoints.Venue;

public static class VendorLogoEndpoints
{
    public static void MapVendorLogoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/vendors/{vendorId:guid}/logo")
            .WithTags("Vendor Logo")
            .RequireAuthorization(policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.HallOwner, RoleConstants.Vendor));

        group.MapPut("/", async (Guid vendorId, IFormFile logo, IMediator mediator, CancellationToken cancellationToken) =>
        {
            await using var stream = logo.OpenReadStream();
            var response = await mediator.Send(new UploadVendorLogoCommand(
                vendorId, stream, logo.FileName, logo.ContentType, logo.Length), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .DisableAntiforgery()
        .WithName("UploadVendorLogo")
        .WithSummary("Upload or Replace Vendor Logo")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<ApiResponse<VendorLogoDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (Guid vendorId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetVendorLogoQuery(vendorId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetVendorLogo")
        .WithSummary("Get Vendor Logo URL")
        .Produces<ApiResponse<VendorLogoDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/content", async (Guid vendorId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetVendorLogoContentQuery(vendorId), cancellationToken);
            return result is null ? Results.NotFound() : Results.Stream(result.Stream, result.ContentType);
        })
        .WithName("GetVendorLogoContent")
        .WithSummary("Get Vendor Logo Content")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/", async (Guid vendorId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteVendorLogoCommand(vendorId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteVendorLogo")
        .WithSummary("Remove Vendor Logo")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
