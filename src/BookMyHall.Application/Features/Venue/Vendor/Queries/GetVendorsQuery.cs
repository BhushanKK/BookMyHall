using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorsQuery(PaginationRequest Request, Guid? AreaId = null)
    : IRequest<ApiResponse<PaginatedResponse<VendorListView>>>;