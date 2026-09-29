using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorPackageCommand : VendorPackageDto, IRequest<ApiResponse<VendorPackageDto>>;
