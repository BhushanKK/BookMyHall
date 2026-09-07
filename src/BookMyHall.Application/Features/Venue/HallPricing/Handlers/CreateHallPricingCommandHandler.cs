using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallPricingCommandHandler(
    IHallPricingRepository hallPricingRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateHallPricingCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        CreateHallPricingCommand,
        ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        CreateHallPricingCommand request,
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
        // MAP REQUEST -> ENTITY
        // =========================================================

        var hallPricing =
            mapper.Map<HallPricing>(request);

        try
        {
            // =====================================================
            // INSERT
            // =====================================================

            await hallPricingRepository.AddAsync(
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
        // CACHE INVALIDATION
        //
        // A new pricing record can affect:
        //
        // 1. All-hall paginated results
        // 2. Hall-specific paginated results
        // 3. Different pages
        // 4. Different page sizes
        // 5. Different searches
        // 6. Different sorting
        //
        // Therefore clear all paginated Hall Pricing caches.
        // =========================================================

        await cacheService.RemoveByPrefixAsync(
            HallPricingCacheKeyBuilder
                .BuildPaginatedPrefix(),
            cancellationToken);

        // =========================================================
        // MAP ENTITY -> DTO
        // =========================================================

        var response =
            mapper.Map<HallPricingDto>(
                hallPricing);

        // =========================================================
        // RETURN SUCCESS
        // =========================================================

        return ApiResponse<HallPricingDto>
            .SuccessResponse(
                response,
                messageHelper.AddedEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallPricing),
                HttpStatusCode.Created);
    }
}