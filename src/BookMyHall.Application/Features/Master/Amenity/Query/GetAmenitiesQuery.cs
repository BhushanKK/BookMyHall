using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed record GetAmenitiesQuery(PaginationRequest paginationRequest)
    : IRequest<ApiResponse<PaginatedResponse<Amenity>>>;
