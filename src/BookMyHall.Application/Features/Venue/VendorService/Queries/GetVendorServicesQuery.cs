using BookMyHall.Contracts.Common;

using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorServicesQuery(PaginationRequest Request,Guid? VendorId,
Guid? VendorCategoryId,Guid? VendorSubCategoryId ): IRequest<ApiResponse<PaginatedResponse<VendorServiceDto>>>;
