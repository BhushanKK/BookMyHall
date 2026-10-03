using BookMyHall.Contracts.Common;

using MediatR;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorServiceByIdQuery(Guid VendorServiceId): IRequest<ApiResponse<VendorServiceDto>>;
