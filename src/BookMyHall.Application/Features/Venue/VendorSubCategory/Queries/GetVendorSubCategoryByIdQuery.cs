using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorSubCategoryByIdQuery(Guid VendorSubCategoryId)
    : IRequest<ApiResponse<VendorSubCategoryDto>>;