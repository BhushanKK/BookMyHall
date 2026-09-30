using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using MediatR;

public sealed record GetVendorServiceAreaByIdQuery(Guid VendorServiceAreaId): 
   IRequest<ApiResponse<VendorServiceAreaDto>>;