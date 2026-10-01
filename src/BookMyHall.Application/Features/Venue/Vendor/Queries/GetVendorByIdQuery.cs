using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorByIdQuery(Guid VendorId): IRequest<ApiResponse<Vendors>>;