using System.Net;

using FluentValidation;
using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallPricingCommandHandler(
    IHallPricingRepository hallPricingRepository,
    IUnitOfWork unitOfWork,
    IValidator<DeleteHallPricingCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        DeleteHallPricingCommand,
        ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteHallPricingCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // VALIDATION
        // =========================================================

        var validationResult =
            await validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var message =
                string.Join(
                    " | ",
                    validationResult.Errors
                        .Select(x => x.ErrorMessage));

            return ApiResponse<bool>
                .FailureResponse(
                    message,
                    HttpStatusCode.BadRequest);
        }

        // =========================================================
        // GET ENTITY
        // =========================================================

        var hallPricing =
            await hallPricingRepository.GetByIdAsync(
                request.HallPricingId,
                cancellationToken);

        if (hallPricing is null)
        {
            return ApiResponse<bool>
                .FailureResponse(
                    messageHelper.NotFoundEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallPricing),
                    HttpStatusCode.NotFound);
        }

        // =========================================================
        // CAPTURE CACHE VALUES BEFORE SOFT DELETE
        // =========================================================

        var hallId =
            hallPricing.HallId;

        var eventCategoryId =
            hallPricing.EventCategoryId;

        // =========================================================
        // SOFT DELETE
        // =========================================================

        hallPricing.IsDeleted = true;

        await hallPricingRepository.UpdateAsync(
            hallPricing,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // =========================================================
        // INVALIDATE BY ID
        // =========================================================

        await cacheService.RemoveAsync(
            HallPricingCacheKeyBuilder
                .BuildByIdKey(
                    request.HallPricingId),
            cancellationToken);

        // =========================================================
        // INVALIDATE BY HALL + EVENT CATEGORY
        // =========================================================

        await cacheService.RemoveAsync(
            HallPricingCacheKeyBuilder
                .BuildByHallAndEventCategoryKey(
                    hallId,
                    eventCategoryId),
            cancellationToken);

        // =========================================================
        // INVALIDATE ALL PAGINATION CACHE
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            HallPricingCacheKeyBuilder
                .BuildPaginatedPrefix(),
            cancellationToken);

        // =========================================================
        // RETURN
        // =========================================================

        return ApiResponse<bool>
            .SuccessResponse(
                true,
                messageHelper.DeletedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallPricing),
                HttpStatusCode.OK);
    }
}