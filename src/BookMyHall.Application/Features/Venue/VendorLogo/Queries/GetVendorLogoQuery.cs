using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorLogoQuery(Guid VendorId) : IRequest<ApiResponse<VendorLogoDto>>;

public sealed record GetVendorLogoContentQuery(Guid VendorId) : IRequest<VendorLogoContentResult?>;
