using System.Net;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallDetailsByIdQueryHandler(
    IHallRepository hallRepository,
    IMessageHelper messageHelper,
    IR2StorageService storageService)
    : IRequestHandler<GetHallDetailsByIdQuery, ApiResponse<HallListView>>
{
    public async Task<ApiResponse<HallListView>> Handle(
        GetHallDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var hall = await hallRepository.GetHallDetailsByIdAsync(
            request.HallId,
            cancellationToken);

        if (hall is null)
        {
            return ApiResponse<HallListView>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Hall),
                HttpStatusCode.NotFound);
        }

        // =========================================================
        // Generate pre-signed URL for cover image
        // =========================================================

        if (!string.IsNullOrWhiteSpace(hall.CoverImageUrl))
        {
            hall.CoverImageUrl =
                await storageService.GetPreSignedUrlAsync(
                    hall.CoverImageUrl,
                    TimeSpan.FromDays(6).Add(
                        TimeSpan.FromHours(23)),
                    cancellationToken);
        }

        return ApiResponse<HallListView>.SuccessResponse(
            hall,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Hall),
            HttpStatusCode.OK);
    }
}