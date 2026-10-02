using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorImageContentQuery(Guid VendorImageId) : IRequest<VendorImageContentResult?>;