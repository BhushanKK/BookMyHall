using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetServicesQueryHandler(
    IServiceRepository serviceRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetServicesQuery, ApiResponse<PaginatedResult<ServiceDto>>>
{
    public async Task<ApiResponse<PaginatedResult<ServiceDto>>> Handle(GetServicesQuery request,CancellationToken cancellationToken)
    {
        var result = await serviceRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResult<ServiceDto>
        {
            Items = mapper.Map<List<ServiceDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<ServiceDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}