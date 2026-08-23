using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteCountryCommandHandler(
    ICountryRepository countryRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<DeleteCountryCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteCountryCommand request,
        CancellationToken cancellationToken)
    {
        var country = await countryRepository.GetByIdAsync(request.CountryId, cancellationToken);

        if (country is null)
        {
            return ApiResponse<bool>.FailureResponse
            (
                messageHelper.NotFound(EntityKeys.Country),
                HttpStatusCode.NotFound
            );
        }

        country.IsActive = false;

        await countryRepository.UpdateAsync(country, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.Country}:{request.CountryId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CountriesPaged}:", cancellationToken);

        return ApiResponse<bool>.SuccessResponse
        (
            true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.Country),
            HttpStatusCode.OK
        );
    }
}