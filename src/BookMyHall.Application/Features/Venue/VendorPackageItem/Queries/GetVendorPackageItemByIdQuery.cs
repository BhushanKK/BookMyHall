using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using MediatR;
public sealed record GetVendorPackageItemByIdQuery(Guid VendorPackageItemId): IRequest<ApiResponse<VendorPackageItemDto>>;