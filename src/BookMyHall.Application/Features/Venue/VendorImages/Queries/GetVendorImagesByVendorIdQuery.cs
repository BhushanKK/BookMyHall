using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorImagesByVendorIdQuery(Guid VendorId, PaginationRequest Pagination, Guid? VendorServiceId = null)
    : IRequest<ApiResponse<PaginatedResult<BookMyHall.Contracts.Venue.VendorImageDto>>>;
