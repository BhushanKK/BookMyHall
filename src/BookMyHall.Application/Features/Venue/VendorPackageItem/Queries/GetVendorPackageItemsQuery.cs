using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;
public sealed record GetVendorPackageItemsQuery(PaginationRequest Pagination, Guid? VendorPackageId)
    : IRequest<ApiResponse<PaginatedResponse<VendorPackageItem>>>;