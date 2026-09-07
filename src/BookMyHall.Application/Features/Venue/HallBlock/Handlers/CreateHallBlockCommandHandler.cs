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

public sealed class CreateHallBlockCommandHandler(
    IHallBlockRepository hallBlockRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateHallBlockCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        CreateHallBlockCommand,
        ApiResponse<HallBlockDto>>
{
    public async Task<ApiResponse<HallBlockDto>> Handle(
        CreateHallBlockCommand request,
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
        // MAP REQUEST -> ENTITY
        // =====================================================

        var hallBlock =
            mapper.Map<HallBlock>(request);

        hallBlock.IsActive = true;


        // =====================================================
        // SAVE
        // =====================================================

        try
        {
            await hallBlockRepository.AddAsync(
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
        // Create affects every paginated result because a new
        // HallBlock may change:
        //
        // - total count
        // - pages
        // - search results
        // - sorting
        // - Hall-filtered results
        // =====================================================

        await cacheService.RemoveByPrefixAsync(
            HallBlockCacheKeyBuilder.BuildPaginatedPrefix(),
            cancellationToken);


        // =====================================================
        // RESPONSE
        // =====================================================

        var response =
            mapper.Map<HallBlockDto>(hallBlock);

        return ApiResponse<HallBlockDto>.SuccessResponse(
            response,
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.HallBlock),
            HttpStatusCode.Created);
    }
}