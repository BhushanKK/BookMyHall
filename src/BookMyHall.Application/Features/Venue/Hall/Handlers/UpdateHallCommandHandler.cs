using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallCommandHandler(IHallRepository hallRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<UpdateHallCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateHallCommand, ApiResponse<HallDto>>
{
    public async Task<ApiResponse<HallDto>> Handle(UpdateHallCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ApiResponse<HallDto>.FailureResponse
            (
                message,
                HttpStatusCode.BadRequest
            );
        }

        var hall = await hallRepository.GetByIdAsync(request.HallId, cancellationToken);

        if (hall is null)
        {
            return ApiResponse<HallDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.NotFound
            );
        }

        mapper.Map(request, hall);

        try
        {
            await hallRepository.UpdateAsync(hall, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Hall),
                HttpStatusCode.Conflict
            );
        }
        await cacheService.RemoveAsync($"{CacheKeys.Hall}:{request.HallId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallsPaged}:", cancellationToken);
        return ApiResponse<HallDto>.SuccessResponse
        (
            mapper.Map<HallDto>(hall),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.Hall),
            HttpStatusCode.OK
        );
    }
}