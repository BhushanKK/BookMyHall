using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using MediatR;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorsQuery(PaginationRequest Request): IRequest<ApiResponse<PaginatedResponse<Vendors>>>;