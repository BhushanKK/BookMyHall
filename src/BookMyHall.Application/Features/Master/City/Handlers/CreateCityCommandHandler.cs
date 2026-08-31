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

public sealed class CreateCityCommandHandler(
    ICityRepository cityRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateCityCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateCityCommand, ApiResponse<CityDto>>
{
    public async Task<ApiResponse<CityDto>> Handle(CreateCityCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<CityDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }
        var city = mapper.Map<City>(request);
        city.CityId = Guid.NewGuid();
        city.IsActive = true;

        try
        {
            await cityRepository.AddAsync(city,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<CityDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.City),HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CitiesPaged}:", cancellationToken);
        return ApiResponse<CityDto>.SuccessResponse(
            mapper.Map<CityDto>(city),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.City),HttpStatusCode.Created);
    }
}