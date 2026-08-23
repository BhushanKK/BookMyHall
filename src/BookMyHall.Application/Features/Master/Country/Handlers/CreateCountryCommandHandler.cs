using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateCountryCommandHandler(
    ICountryRepository countryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateCountryCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<CreateCountryCommand, ApiResponse<CountryDto>>
{
    public async Task<ApiResponse<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<CountryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var country = mapper.Map<Country>(request);

        country.CountryId = Guid.NewGuid();
        country.IsActive = true;

        try
        {
            await countryRepository.AddAsync(country,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<CountryDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.Country),
                HttpStatusCode.Conflict
            );
        }

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CountriesPaged}:", cancellationToken);

        return ApiResponse<CountryDto>.SuccessResponse
        (
            mapper.Map<CountryDto>(country),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Country),
            HttpStatusCode.Created
        );
    }
}