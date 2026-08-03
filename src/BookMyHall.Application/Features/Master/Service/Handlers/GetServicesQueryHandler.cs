using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetServicesQueryHandler(
    IServiceRepository serviceRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetServicesQuery, ApiResponse<PaginatedResult<Service>>>
{
    public async Task<ApiResponse<PaginatedResult<Service>>> Handle(GetServicesQuery request,CancellationToken cancellationToken)
    {
        var result = await serviceRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResult<Service>
        {
            Items = mapper.Map<IReadOnlyList<Service>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<Service>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}