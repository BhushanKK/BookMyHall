using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;
public sealed record GetVendorPackageItemByIdQuery(Guid VendorPackageItemId): IRequest<ApiResponse<VendorPackageItem>>;