using MediatR;
using BookMyHall.Domain.Dtos;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
namespace BookMyHall.Application.Features.HallOwner.Queries;

public sealed class GetHallOwnersQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetHallOwnersQuery, IReadOnlyList<HallOwnerDto>>
{
    public async Task<IReadOnlyList<HallOwnerDto>> Handle(
        GetHallOwnersQuery request, CancellationToken cancellationToken)
        => await userRepository.GetHallOwnersAsync
        (request.SearchText, cancellationToken);
}