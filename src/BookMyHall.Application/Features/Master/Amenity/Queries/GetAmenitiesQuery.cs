using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetAmenitiesQuery(
    PaginationRequest Request)
    : IRequest<ApiResponse<PaginatedResponse<AmenityDto>>>;
