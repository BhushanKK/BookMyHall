using BookMyHall.Contracts.Common;
using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorByIdQuery(Guid VendorId): IRequest<ApiResponse<VendorDto>>;