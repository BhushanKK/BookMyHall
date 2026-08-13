using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallByIdQueryHandler(
    IHallRepository hallRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetHallByIdQuery, ApiResponse<Hall>>
{
    public async Task<ApiResponse<Hall>> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
    {
        var hall = await hallRepository.GetByIdAsync(request.HallId, cancellationToken);

        if (hall is null)
        {
            return ApiResponse<Hall>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Hall),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Hall>.SuccessResponse
        (
            mapper.Map<Hall>(hall),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}