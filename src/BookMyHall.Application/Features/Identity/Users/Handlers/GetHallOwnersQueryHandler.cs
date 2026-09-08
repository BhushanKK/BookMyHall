using MediatR;
using BookMyHall.Domain.Constants;
using BookMyHall.Domain.Dtos;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;

namespace BookMyHall.Application.Features.HallOwner.Queries;

public sealed class GetHallOwnersQueryHandler(
    IUserRepository userRepository, ICurrentUser currentUser)
    : IRequestHandler<GetHallOwnersQuery, IReadOnlyList<HallOwnerDto>>
{
    public async Task<IReadOnlyList<HallOwnerDto>> Handle(
        GetHallOwnersQuery request,
        CancellationToken cancellationToken)
    {
        var isAdmin = currentUser.Roles.Any(role => string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase));
        var isHallOwner = currentUser.Roles.Any(role => string.Equals(role, RoleConstants.HallOwner, StringComparison.OrdinalIgnoreCase));

        Guid? hallOwnerId = null;

        if (isHallOwner && !isAdmin)
        {
            if (currentUser.UserId is null)
                return [];

            hallOwnerId = currentUser.UserId.Value;
        }
        var searchText = request.SearchText?.Trim() ?? string.Empty;

        IReadOnlyList<HallOwnerDto> result;

        if (hallOwnerId.HasValue)
            result = await userRepository.GetHallOwnersAsync(searchText, hallOwnerId.Value, cancellationToken);
        else
            result = await userRepository.GetHallOwnersAsync(searchText, null, cancellationToken);

        return result;
    }
}