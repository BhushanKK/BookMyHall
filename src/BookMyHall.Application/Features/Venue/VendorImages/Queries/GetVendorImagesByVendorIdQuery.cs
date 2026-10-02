using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorImagesByVendorIdQuery(Guid VendorId, PaginationRequest Pagination)
    : IRequest<ApiResponse<PaginatedResult<BookMyHall.Contracts.Venue.VendorImageDto>>>;