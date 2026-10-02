using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record DeleteVendorImageCommand(Guid VendorImageId) : IRequest<ApiResponse<bool>>;