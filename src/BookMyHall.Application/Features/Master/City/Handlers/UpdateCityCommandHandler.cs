using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCityCommandHandler(
    ICityRepository cityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateCityCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateCityCommand, ApiResponse<CityDto>>
{
    public async Task<ApiResponse<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<CityDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);

        if (city is null)
        {
            return ApiResponse<CityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.City),
                HttpStatusCode.NotFound);
        }

        var existingCity = await cityRepository.GetByCityNameAsync(request.CityName, cancellationToken);

        if (existingCity is not null && existingCity.CityId != request.CityId)
        {
            return ApiResponse<CityDto>.FailureResponse(messageHelper.AlreadyExists(EntityKeys.City), HttpStatusCode.BadRequest);
        }

        mapper.Map(request, city);
        await cityRepository.UpdateAsync(city, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Cities}:{request.CityId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CitiesPaged}:", cancellationToken);
        return ApiResponse<CityDto>.SuccessResponse(
            mapper.Map<CityDto>(city),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.City), HttpStatusCode.OK);
    }
}