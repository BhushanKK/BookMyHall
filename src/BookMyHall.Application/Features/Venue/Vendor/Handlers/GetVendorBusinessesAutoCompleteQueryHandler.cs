using System.Net;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorBusinessesAutoCompleteQueryHandler(
    IVendorRepository vendorRepository,
    IMessageHelper messageHelper)
    : IRequestHandler<GetVendorBusinessesAutoCompleteQuery, ApiResponse<IReadOnlyList<AutoCompleteItem>>>
{
    public async Task<ApiResponse<IReadOnlyList<AutoCompleteItem>>> Handle(
        GetVendorBusinessesAutoCompleteQuery request,
        CancellationToken cancellationToken)
    {
        var vendor = await vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null)
        {
            return ApiResponse<IReadOnlyList<AutoCompleteItem>>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound);
        }

        var items = await vendorRepository.GetBusinessAutoCompleteAsync(
            request.VendorId,
            request.SearchTerm,
            request.AreaId,
            Math.Clamp(request.Limit, 1, 30),
            cancellationToken);

        return ApiResponse<IReadOnlyList<AutoCompleteItem>>.SuccessResponse(
            items,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.OK);
    }
}