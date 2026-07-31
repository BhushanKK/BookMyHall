using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateAmenityCommandHandler(
    IAmenityRepository amenityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<UpdateAmenityCommand, ApiResponse<AmenityDto>>
{
    public async Task<ApiResponse<AmenityDto>> Handle(
        UpdateAmenityCommand request,
        CancellationToken cancellationToken)
    {
        var amenity = await amenityRepository.GetByIdAsync(
            request.AmenityId,
            cancellationToken);

        if (amenity is null)
        {
            return ApiResponse<AmenityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Amenity),
                HttpStatusCode.NotFound);
        }

        var existingAmenity = await amenityRepository.GetByAmenityNameAsync(
            request.AmenityName,
            cancellationToken);

        if (existingAmenity is not null &&
            existingAmenity.AmenityId != request.AmenityId)
        {
            return ApiResponse<AmenityDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Amenity),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, amenity);

        await amenityRepository.UpdateAsync(amenity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var amenityDto = mapper.Map<AmenityDto>(amenity);

        return ApiResponse<AmenityDto>.SuccessResponse(
            amenityDto,
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.Amenity),
            HttpStatusCode.OK);
    }
}