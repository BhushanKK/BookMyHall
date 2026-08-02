using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAmenityQueryHandler(
    IAmenityRepository amenityRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetAmenitiesQuery, ApiResponse<PaginatedResponse<Amenity>>>
{
    public async Task<ApiResponse<PaginatedResponse<Amenity>>> Handle(GetAmenitiesQuery request,CancellationToken cancellationToken)
    {
        var pagedResult = await amenityRepository.GetAllAsync(request.Request, cancellationToken);

        var response = new PaginatedResponse<Amenity>
        {
            Items = mapper.Map<IReadOnlyList<Amenity>>(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalRecords = pagedResult.TotalCount
        };

        return ApiResponse<PaginatedResponse<Amenity>>.SuccessResponse
        (
            response,
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Amenity),
            HttpStatusCode.OK
        );
    }
}