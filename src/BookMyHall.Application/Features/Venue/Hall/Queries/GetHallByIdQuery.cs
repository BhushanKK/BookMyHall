using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallByIdQuery(Guid HallId) : IRequest<ApiResponse<Hall>>;