using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorByIdQuery(Guid VendorId) : IRequest<ApiResponse<Vendor>>;