using System.Net;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetHallByHallNameAndAreaQueryHandler(
    IHallRepository hallRepository,IMessageHelper messageHelper)
    : IRequestHandler<GetHallByHallNameAndAreaQuery, ApiResponse<Hall>>
{
    public async Task<ApiResponse<Hall>> Handle(
        GetHallByHallNameAndAreaQuery request,
        CancellationToken cancellationToken)
    {
        var hall = await hallRepository.GetByHallNameAndAreaAsync(
            request.HallName,
            request.AreaId,
            cancellationToken);

        if (hall is null)
        {
            return ApiResponse<Hall>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Hall>.SuccessResponse
        (
            hall,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}