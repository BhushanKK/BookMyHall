using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorAvailabilitiesQuery(PaginationRequest Request,Guid? VendorId)
    : IRequest<ApiResponse<PaginatedResponse<VendorAvailability>>>;