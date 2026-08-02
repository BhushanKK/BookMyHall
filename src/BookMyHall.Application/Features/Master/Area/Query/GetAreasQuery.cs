using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed record GetAreasQuery(PaginationRequest paginationRequest)
    : IRequest<ApiResponse<PaginatedResult<Area>>>;
