using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorSubCategoryByIdQuery(Guid VendorSubCategoryId)
    : IRequest<ApiResponse<VendorSubCategory>>;