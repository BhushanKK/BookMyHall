using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using MediatR;

public sealed record GetVendorServiceAreasQuery(PaginationRequest Pagination,
    Guid? VendorId): IRequest<ApiResponse<PaginatedResponse<VendorServiceAreaDto>>>;