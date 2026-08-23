using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallBlockCommandHandler(IHallBlockRepository hallBlockRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<UpdateHallBlockCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateHallBlockCommand,ApiResponse<HallBlockDto>>
{
    public async Task<ApiResponse<HallBlockDto>> Handle(UpdateHallBlockCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<HallBlockDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var hallBlock = await hallBlockRepository.GetByIdAsync(request.HallBlockId,cancellationToken);

        if (hallBlock is null)
        {
            return ApiResponse<HallBlockDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,
                    EntityKeys.HallBlock),HttpStatusCode.NotFound);
        }

        mapper.Map(request, hallBlock);

        try
        {
            await hallBlockRepository.UpdateAsync(hallBlock,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallBlockDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.HallBlock),HttpStatusCode.Conflict);
        }
        
        await cacheService.RemoveAsync($"{CacheKeys.HallBlock}:{request.HallBlockId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallBlocksPaged}:", cancellationToken);

        return ApiResponse<HallBlockDto>.SuccessResponse(
            mapper.Map<HallBlockDto>(hallBlock),
            messageHelper.UpdatedEntity(ResourceNames.Entities,
                EntityKeys.HallBlock),HttpStatusCode.OK);
    }
}