using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallByHallNameAndAreaQuery(string HallName,Guid AreaId)
    : IRequest<ApiResponse<Hall>>;