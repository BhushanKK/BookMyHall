using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateAmenityCommandHandler(
    IAmenityRepository amenityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateAmenityCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(
        CreateAmenityCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await amenityRepository.GetByAmenityNameAsync(
            request.AmenityName,
            cancellationToken);

        if (existing is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Amenity),
                HttpStatusCode.BadRequest);
        }

        var amenity = mapper.Map<Amenity>(request);

        amenity.AmenityId = Guid.NewGuid();
        amenity.IsActive = true;

        await amenityRepository.AddAsync(amenity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(
            amenity.AmenityId,
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.Amenity),
            HttpStatusCode.Created);
    }
}