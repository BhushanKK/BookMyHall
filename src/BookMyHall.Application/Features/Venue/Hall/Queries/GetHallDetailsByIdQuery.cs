using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallDetailsByIdQuery(Guid HallId) 
    : IRequest<ApiResponse<HallListView>>;