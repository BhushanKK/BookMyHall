using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorAvailabilityCommand: VendorAvailabilityDto, IRequest<ApiResponse<VendorAvailabilityDto>>;