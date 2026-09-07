using System.Net;

using AutoMapper;
using FluentValidation;
using MediatR;

using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallPricingCommandHandler(
    IHallPricingRepository hallPricingRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateHallPricingCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        UpdateHallPricingCommand,
        ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        UpdateHallPricingCommand request,
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

            return ApiResponse<HallPricingDto>
                .FailureResponse(
                    message,
                    HttpStatusCode.BadRequest);
        }

        // =========================================================
        // GET EXISTING ENTITY
        // =========================================================

        var hallPricing =
            await hallPricingRepository.GetByIdAsync(
                request.HallPricingId,
                cancellationToken);

        if (hallPricing is null)
        {
            return ApiResponse<HallPricingDto>
                .FailureResponse(
                    messageHelper.NotFoundEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallPricing),
                    HttpStatusCode.NotFound);
        }

        // =========================================================
        // CAPTURE OLD CACHE VALUES
        //
        // Required because HallId/EventCategoryId could change.
        // =========================================================

        var oldHallId =
            hallPricing.HallId;

        var oldEventCategoryId =
            hallPricing.EventCategoryId;

        // =========================================================
        // MAP REQUEST -> EXISTING ENTITY
        // =========================================================

        mapper.Map(
            request,
            hallPricing);

        try
        {
            // =====================================================
            // UPDATE DATABASE
            // =====================================================

            await hallPricingRepository.UpdateAsync(
                hallPricing,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallPricingDto>
                .FailureResponse(
                    messageHelper.AlreadyExistsEntity(
                        ResourceNames.Entities,
                        EntityKeys.HallPricing),
                    HttpStatusCode.Conflict);
        }

        // =========================================================
        // INVALIDATE BY ID
        // =========================================================

        await cacheService.RemoveAsync(
            HallPricingCacheKeyBuilder
                .BuildByIdKey(
                    request.HallPricingId),
            cancellationToken);

        // =========================================================
        // INVALIDATE OLD HALL + EVENT CATEGORY CACHE
        // =========================================================

        await cacheService.RemoveAsync(
            HallPricingCacheKeyBuilder
                .BuildByHallAndEventCategoryKey(
                    oldHallId,
                    oldEventCategoryId),
            cancellationToken);

        // =========================================================
        // INVALIDATE NEW HALL + EVENT CATEGORY CACHE
        //
        // Important if HallId/EventCategoryId changed.
        // =========================================================

        await cacheService.RemoveAsync(
            HallPricingCacheKeyBuilder
                .BuildByHallAndEventCategoryKey(
                    hallPricing.HallId,
                    hallPricing.EventCategoryId),
            cancellationToken);

        // =========================================================
        // INVALIDATE ALL PAGINATED CACHE
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            HallPricingCacheKeyBuilder
                .BuildPaginatedPrefix(),
            cancellationToken);

        // =========================================================
        // MAP RESPONSE
        // =========================================================

        var response =
            mapper.Map<HallPricingDto>(
                hallPricing);

        // =========================================================
        // RETURN
        // =========================================================

        return ApiResponse<HallPricingDto>
            .SuccessResponse(
                response,
                messageHelper.UpdatedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallPricing),
                HttpStatusCode.OK);
    }
}