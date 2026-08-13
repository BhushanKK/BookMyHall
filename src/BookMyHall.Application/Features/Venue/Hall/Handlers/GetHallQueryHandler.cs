using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetHallQueryHandler(
    IHallRepository hallRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetHallQuery, ApiResponse<PaginatedResult<Hall>>>
{
    public async Task<ApiResponse<PaginatedResult<Hall>>> Handle(GetHallQuery request,
        CancellationToken cancellationToken)
    {
        var result = await hallRepository.GetAllAsync(request.paginationRequest, cancellationToken);

        var response = new PaginatedResult<Hall>
        {
            Items = mapper.Map<IReadOnlyList<Hall>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<Hall>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}