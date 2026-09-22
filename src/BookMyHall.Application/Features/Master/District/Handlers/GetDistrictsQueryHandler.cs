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
public sealed class GetDistrictsQueryHandler(IDistrictRepository districtRepository,
    IMessageHelper messageHelper,IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetDistrictsQuery, ApiResponse<PaginatedResponse<District>>>
{
    public async Task<ApiResponse<PaginatedResponse<District>>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<District>(
            CacheKeys.DistrictsPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<District>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<District>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.District),
                HttpStatusCode.OK
            );
        }
        var result = await districtRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResponse<District>
        {
            Items = mapper.Map<IReadOnlyList<District>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<District>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.District), HttpStatusCode.OK);
    }
}