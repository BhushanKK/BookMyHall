using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorImageByIdQuery(Guid VendorImageId) : IRequest<ApiResponse<VendorImageDto>>;