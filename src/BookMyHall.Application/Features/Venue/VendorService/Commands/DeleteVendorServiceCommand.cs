using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteVendorServiceCommand(Guid VendorServiceId): IRequest<ApiResponse<bool>>;