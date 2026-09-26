using BookMyHall.Contracts.Common;
using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorsQuery(PaginationRequest Request): IRequest<ApiResponse<PaginatedResult<VendorDto>>>;