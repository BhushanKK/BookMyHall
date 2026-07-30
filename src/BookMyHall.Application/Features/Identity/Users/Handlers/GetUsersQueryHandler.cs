using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class GetUsersQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetUsersQuery, ApiResponse<PaginatedResponse<UserDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await userRepository.GetAllAsync(request.Request, cancellationToken);

        var response = new PaginatedResponse<UserDto>
        {
            Items = mapper.Map<IReadOnlyList<UserDto>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.User),
            HttpStatusCode.OK
        );
    }
}