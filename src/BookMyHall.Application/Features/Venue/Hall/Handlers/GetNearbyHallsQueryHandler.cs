using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Common.Interfaces.Storage;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetNearbyHallsQueryHandler(
    IHallRepository hallRepository,
    IR2StorageService storageService,
    IMessageHelper messageHelper)
    : IRequestHandler<
        GetNearbyHallsQuery,
        ApiResponse<PaginatedResult<NearbyHallView>>>
{
    public async Task<ApiResponse<PaginatedResult<NearbyHallView>>> Handle(
        GetNearbyHallsQuery request,
        CancellationToken cancellationToken)
    {
        // ============================================================
        // VALIDATE LATITUDE
        // ============================================================

        if (request.Latitude < -90 || request.Latitude > 90)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Latitude must be between -90 and 90.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE LONGITUDE
        // ============================================================

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Longitude must be between -180 and 180.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE RADIUS
        // ============================================================

        if (request.RadiusKm <= 0)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Radius must be greater than 0.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE PAGE NUMBER
        // ============================================================

        if (request.PageNumber <= 0)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Page number must be greater than 0.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // VALIDATE PAGE SIZE
        // ============================================================

        if (request.PageSize <= 0)
        {
            return ApiResponse<PaginatedResult<NearbyHallView>>.FailureResponse(
                "Page size must be greater than 0.",
                HttpStatusCode.BadRequest);
        }

        // ============================================================
        // CREATE PAGINATION REQUEST
        // ============================================================

        var paginationRequest = new PaginationRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        // ============================================================
        // GET NEARBY HALLS
        // ============================================================

        var result = await hallRepository.GetNearbyAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            paginationRequest,
            cancellationToken);

        // ============================================================
        // GENERATE R2 PRESIGNED URL FOR COVER IMAGE
        // ============================================================

        foreach (var hall in result.Items)
        {
            if (!string.IsNullOrWhiteSpace(hall.CoverImageUrl))
            {
                hall.CoverImageUrl =
                    await storageService.GetPreSignedUrlAsync(
                        hall.CoverImageUrl,
                        TimeSpan.FromDays(6).Add(
                            TimeSpan.FromHours(23)),
                        cancellationToken);
            }
        }

        // ============================================================
        // RETURN RESPONSE
        // ============================================================

        return ApiResponse<PaginatedResult<NearbyHallView>>.SuccessResponse(
            result,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Hall),
            HttpStatusCode.OK);
    }
}