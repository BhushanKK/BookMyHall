using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallDetailsByIdQueryHandler(
    IHallRepository hallRepository,
    IMessageHelper messageHelper)
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

        return ApiResponse<HallListView>.SuccessResponse(
            hall,
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Hall),
            HttpStatusCode.OK);
    }
}