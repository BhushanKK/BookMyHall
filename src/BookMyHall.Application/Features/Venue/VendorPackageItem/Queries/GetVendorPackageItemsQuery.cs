using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using MediatR;
public sealed record GetVendorPackageItemsQuery(PaginationRequest Pagination, Guid? VendorPackageId)
    : IRequest<ApiResponse<PaginatedResponse<VendorPackageItemDto>>>;