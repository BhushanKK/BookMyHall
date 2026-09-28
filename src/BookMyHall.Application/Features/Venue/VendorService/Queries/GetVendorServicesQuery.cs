using BookMyHall.Contracts.Common;
using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorServicesQuery(PaginationRequest Request,Guid? VendorId,Guid? VendorSubCategoryId ): IRequest<ApiResponse<PaginatedResponse<VendorServiceDto>>>;