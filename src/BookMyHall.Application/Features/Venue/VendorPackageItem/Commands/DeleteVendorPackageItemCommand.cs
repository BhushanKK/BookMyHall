using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteVendorPackageItemCommand(Guid VendorPackageItemId) : IRequest<ApiResponse<bool>>;
