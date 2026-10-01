using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

using MediatR;

public sealed record GetVendorServiceAreaByIdQuery(Guid VendorServiceAreaId): 
   IRequest<ApiResponse<VendorServiceArea>>;