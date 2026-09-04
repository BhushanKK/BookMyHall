using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallQuery(PaginationRequest paginationRequest)
    : IRequest<ApiResponse<PaginatedResult<HallListDto>>>;