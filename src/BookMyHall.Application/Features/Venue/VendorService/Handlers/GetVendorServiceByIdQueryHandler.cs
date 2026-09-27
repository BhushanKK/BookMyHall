using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class GetVendorServiceByIdQueryHandler(IVendorServiceRepository vendorServiceRepository,
    IMapper mapper,IMessageHelper messageHelper)
    : IRequestHandler<GetVendorServiceByIdQuery, ApiResponse<VendorServiceDto>>
{
    public async Task<ApiResponse<VendorServiceDto>> Handle(GetVendorServiceByIdQuery request,CancellationToken cancellationToken)
    {
        var vendorService = await vendorServiceRepository.GetByIdAsync(request.VendorServiceId,cancellationToken);

        if (vendorService is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorService),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<VendorServiceDto>.SuccessResponse(mapper.Map<VendorServiceDto>(vendorService),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.OK);
    }
}