using BookMyHall.Domain.Dtos;
using MediatR;

namespace BookMyHall.Application.Features.HallOwner.Queries;

public sealed record GetHallOwnersQuery(
    string? SearchText = null)
    : IRequest<IReadOnlyList<HallOwnerDto>>;