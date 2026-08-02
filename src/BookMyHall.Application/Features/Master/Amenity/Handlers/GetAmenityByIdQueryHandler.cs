using MediatR;
using System.Net;
using AutoMapper;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetAmenityByIdQueryHandler(
    IAmenityRepository amenityRepository,
    IMapper mapper,
    IMessageHelper messageHelper)
    : IRequestHandler<GetAmenityByIdQuery, ApiResponse<Amenity>>
{
    public async Task<ApiResponse<Amenity>> Handle(GetAmenityByIdQuery request,CancellationToken cancellationToken)
    {
        var Amenity = await amenityRepository.GetByIdAsync(request.AmenityId,cancellationToken);
        if (Amenity is null)
        {
            return ApiResponse<Amenity>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Amenity),
                HttpStatusCode.NotFound
            );
        }

        return ApiResponse<Amenity>.SuccessResponse
        (
            mapper.Map<Amenity>(Amenity),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.Amenity),
            HttpStatusCode.OK
        );
    }
}