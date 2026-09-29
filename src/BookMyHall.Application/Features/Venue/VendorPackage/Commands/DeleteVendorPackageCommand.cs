using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteVendorPackageCommand(Guid VendorPackageId) : IRequest<ApiResponse<bool>>;
