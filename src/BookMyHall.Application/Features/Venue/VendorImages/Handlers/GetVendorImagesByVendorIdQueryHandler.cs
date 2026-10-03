using System.Net;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Venue;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed class GetVendorImagesByVendorIdQueryHandler(
    IVendorImageRepository repository,
    IR2StorageService storage,
    IMessageHelper messageHelper)
    : IRequestHandler<GetVendorImagesByVendorIdQuery, ApiResponse<PaginatedResult<VendorImageDto>>>
{
    public async Task<ApiResponse<PaginatedResult<VendorImageDto>>> Handle(
        GetVendorImagesByVendorIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Pagination.PageNumber <= 0 || request.Pagination.PageSize <= 0)
        {
            return ApiResponse<PaginatedResult<VendorImageDto>>.FailureResponse(
                "Page number and page size must be greater than zero.",
                HttpStatusCode.BadRequest);
        }

        var result = await repository.GetByVendorIdAsync(
            request.VendorId,
            request.Pagination,
            cancellationToken,
            request.VendorServiceId,
            request.VendorCategoryId,
            request.VendorSubCategoryId);

        var items = new List<VendorImageDto>(result.Items.Count);
        foreach (var image in result.Items)
        {
            items.Add(await VendorImageDtoFactory.CreateAsync(image, storage, cancellationToken));
        }

        var response = new PaginatedResult<VendorImageDto>
        {
            Items = items,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return ApiResponse<PaginatedResult<VendorImageDto>>.SuccessResponse(
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities, EntityKeys.VendorImage),
            HttpStatusCode.OK);
    }
}
