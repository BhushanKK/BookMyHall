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
public sealed class GetStateQueryHandler(IStateRepository stateRepository,IMessageHelper messageHelper,
    IMapper mapper, ICacheService cacheService)
    : IRequestHandler<GetStateQuery, ApiResponse<PaginatedResponse<State>>>
{
    public async Task<ApiResponse<PaginatedResponse<State>>> Handle(GetStateQuery request, CancellationToken cancellationToken)
    {
        var pagination = request.paginationRequest;
        var cacheKey = CacheKeyBuilder.BuildPaginatedKey<State>(
            CacheKeys.StatesPaged,
            pagination.PageNumber,
            pagination.PageSize,
            pagination.SearchText,
            pagination.SortBy,
            pagination.SortDescending);

        var cachedResponse = await cacheService.GetAsync<PaginatedResponse<State>>(cacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return ApiResponse<PaginatedResponse<State>>.SuccessResponse
            (
                cachedResponse,
                messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.State),
                HttpStatusCode.OK
            );
        }
        var result = await stateRepository.GetAllAsync(request.paginationRequest, cancellationToken);
        var response = new PaginatedResponse<State>
        {
            Items = mapper.Map<IReadOnlyList<State>>(result.Items),
            TotalRecords = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);
        return ApiResponse<PaginatedResponse<State>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.State), HttpStatusCode.OK);
    }
}