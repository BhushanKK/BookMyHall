using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteVendorCommand(Guid VendorId): IRequest<ApiResponse<bool>>;