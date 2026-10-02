using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorCoverImageQuery(Guid VendorId) : IRequest<ApiResponse<VendorImageDto>>;