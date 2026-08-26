using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class GetServicesQueryHandler(IServiceRepository serviceRepository,
    IMessageHelper messageHelper,IMapper mapper,ICacheService cacheService)
    : IRequestHandler<GetServicesQuery, ApiResponse<PaginatedResult<Service>>>
{
    public async Task<ApiResponse<PaginatedResult<Service>>> Handle(GetServicesQuery request,CancellationToken cancellationToken)
    {
         var pagination = request.paginationRequest;

        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<Service>(
            CacheKeys.ServicesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResult<Service>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResult<Service>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Service),
                HttpStatusCode.OK
            );
        }
        var result = await serviceRepository.GetAllAsync(request.paginationRequest,cancellationToken);

        var response = new PaginatedResult<Service>
        {
            Items = mapper.Map<IReadOnlyList<Service>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResult<Service>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Service),HttpStatusCode.OK);
    }
}