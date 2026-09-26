using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteVendorAvailabilityCommand(Guid VendorAvailabilityId): IRequest<ApiResponse<bool>>;