using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record DeleteVendorLogoCommand(Guid VendorId) : IRequest<ApiResponse<bool>>;
