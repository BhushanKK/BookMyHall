using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record DeleteVendorServiceAreaCommand(Guid VendorServiceAreaId) : IRequest<ApiResponse<bool>>;
