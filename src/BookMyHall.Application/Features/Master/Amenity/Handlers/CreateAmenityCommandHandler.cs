using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateAmenityCommandHandler(
    IAmenityRepository amenityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateAmenityCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<CreateAmenityCommand, ApiResponse<AmenityDto>>
{
    public async Task<ApiResponse<AmenityDto>> Handle(CreateAmenityCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(
                " | ",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<AmenityDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        var amenity = mapper.Map<Amenity>(request);
        amenity.AmenityId = Guid.NewGuid();
        amenity.IsActive = true;
        try
        {
            await amenityRepository.AddAsync(amenity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<AmenityDto>.FailureResponse(
            messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Amenity), HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.AmenitiesPaged}:", cancellationToken);
        return ApiResponse<AmenityDto>.SuccessResponse(
            mapper.Map<AmenityDto>(amenity),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Amenity), HttpStatusCode.Created);
    }
}