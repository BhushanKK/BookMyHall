using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class GetUserPreferenceQueryHandler(
    IUserPreferenceRepository userPreferenceRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetUserPreferenceQuery,ApiResponse<PaginatedResponse<UserPreferenceDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<UserPreferenceDto>>> Handle(
        GetUserPreferenceQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await userPreferenceRepository.GetAllAsync(request.Request,cancellationToken);
        var response = new PaginatedResponse<UserPreferenceDto>
        {
            Items = mapper.Map<IReadOnlyList<UserPreferenceDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<UserPreferenceDto>>.SuccessResponse(response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,
                EntityKeys.UserPreference),HttpStatusCode.OK);
    }
}