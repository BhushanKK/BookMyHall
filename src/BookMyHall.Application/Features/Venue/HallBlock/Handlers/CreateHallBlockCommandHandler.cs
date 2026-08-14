using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
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
    IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<CreateHallBlockCommand> validator,
    IMessageHelper messageHelper): IRequestHandler<CreateHallBlockCommand,ApiResponse<HallBlockDto>>
{
    public async Task<ApiResponse<HallBlockDto>> Handle(CreateHallBlockCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<HallBlockDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var hallBlock = mapper.Map<HallBlock>(request);
        hallBlock.IsActive = true;
        try
        {
            await hallBlockRepository.AddAsync(hallBlock,cancellationToken);
            await unitOfWork.SaveChangesAsync( cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallBlockDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.HallBlock),HttpStatusCode.Conflict);
        }

        return ApiResponse<HallBlockDto>.SuccessResponse(
            mapper.Map<HallBlockDto>(hallBlock),
            messageHelper.AddedEntity(ResourceNames.Entities,
                EntityKeys.HallBlock),HttpStatusCode.Created);
    }
}