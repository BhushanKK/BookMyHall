using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCountryCommandHandler(
    ICountryRepository countryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateCountryCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<UpdateCountryCommand, ApiResponse<CountryDto>>
{
    public async Task<ApiResponse<CountryDto>> Handle(
        UpdateCountryCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<CountryDto>.FailureResponse
            (
                message,
                HttpStatusCode.BadRequest
            );
        }

        var country = await countryRepository.GetByIdAsync(request.CountryId,cancellationToken);

        if (country is null)
        {
            return ApiResponse<CountryDto>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Country),
                HttpStatusCode.NotFound
            );
        }

        var existingCountry = await countryRepository.GetByCountryNameAsync(request.CountryName, cancellationToken);

        if (existingCountry is not null && existingCountry.CountryId != request.CountryId)
        {
            return ApiResponse<CountryDto>.FailureResponse
            (
                messageHelper.AlreadyExists(EntityKeys.Country),
                HttpStatusCode.BadRequest
            );
        }

        mapper.Map(request, country);

        await countryRepository.UpdateAsync(country, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Country}:", cancellationToken);

        return ApiResponse<CountryDto>.SuccessResponse
        (
            mapper.Map<CountryDto>(country),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.Country),
            HttpStatusCode.OK
        );
    }
}