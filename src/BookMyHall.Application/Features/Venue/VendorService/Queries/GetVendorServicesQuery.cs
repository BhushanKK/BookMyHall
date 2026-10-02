using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorServicesQuery(PaginationRequest Request,Guid? VendorId,
Guid? VendorCategoryId,Guid? VendorSubCategoryId ): IRequest<ApiResponse<PaginatedResponse<VendorService>>>;