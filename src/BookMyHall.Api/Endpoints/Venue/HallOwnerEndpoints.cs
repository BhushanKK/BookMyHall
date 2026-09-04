using System.Net;
using BookMyHall.Application.Features.HallOwner.Queries;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;
using MediatR;

namespace BookMyHall.Api.Endpoints;

public static class HallOwnerEndpoints
{
    public static IEndpointRouteBuilder MapHallOwnerEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/hall-owners")
            .WithTags("Hall Owners")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapGet(
                "/",
                async (
                    string? searchText,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new GetHallOwnersQuery(searchText),
                        cancellationToken);

                    return Results.Ok(
                        ApiResponse<IReadOnlyList<HallOwnerDto>>.SuccessResponse(
                            result,
                            "Hall owners retrieved successfully.",
                            HttpStatusCode.OK));
                })
            .WithName("GetHallOwners")
            .Produces<ApiResponse<IReadOnlyList<HallOwnerDto>>>(StatusCodes.Status200OK);

        return endpoints;
    }
}