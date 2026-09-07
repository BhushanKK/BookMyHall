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

public sealed class UpdateHallBlockCommandHandler(
    IHallBlockRepository hallBlockRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateHallBlockCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        UpdateHallBlockCommand,
        ApiResponse<HallBlockDto>>
{
    public async Task<ApiResponse<HallBlockDto>> Handle(
        UpdateHallBlockCommand request,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // VALIDATION
        // =====================================================

        var validationResult =
            await validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var message =
                string.Join(
                    " | ",
                    validationResult.Errors.Select(
                        x => x.ErrorMessage));

            return ApiResponse<HallBlockDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }


        // =====================================================
        // GET EXISTING RECORD
        // =====================================================

        var hallBlock =
            await hallBlockRepository.GetByIdAsync(
                request.HallBlockId,
                cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<HallBlockDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallBlock),
                HttpStatusCode.NotFound);
        }


        // =====================================================
        // MAP REQUEST -> EXISTING ENTITY
        // =====================================================

        mapper.Map(
            request,
            hallBlock);


        // =====================================================
        // SAVE
        // =====================================================

        try
        {
            await hallBlockRepository.UpdateAsync(
                hallBlock,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallBlockDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.HallBlock),
                HttpStatusCode.Conflict);
        }


        // =====================================================
        // CACHE INVALIDATION
        //
        // Individual record
        // =====================================================

        await cacheService.RemoveAsync(
            HallBlockCacheKeyBuilder.BuildByIdKey(
                request.HallBlockId),
            cancellationToken);


        // =====================================================
        // CACHE INVALIDATION
        //
        // Every paginated cache may be affected because
        // HallBlock fields may have changed.
        //
        // This also handles HallId changes.
        // =====================================================

        await cacheService.RemoveByPrefixAsync(
            HallBlockCacheKeyBuilder.BuildPaginatedPrefix(),
            cancellationToken);


        // =====================================================
        // RESPONSE
        // =====================================================

        var response =
            mapper.Map<HallBlockDto>(
                hallBlock);

        return ApiResponse<HallBlockDto>.SuccessResponse(
            response,
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.HallBlock),
            HttpStatusCode.OK);
    }
}