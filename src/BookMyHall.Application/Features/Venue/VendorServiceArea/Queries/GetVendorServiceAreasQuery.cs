using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;

public sealed record GetVendorServiceAreasQuery(PaginationRequest Pagination,
    Guid? VendorId): IRequest<ApiResponse<PaginatedResponse<VendorServiceArea>>>;