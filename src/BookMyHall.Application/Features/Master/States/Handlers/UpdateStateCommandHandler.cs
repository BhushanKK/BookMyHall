using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Features.Master;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateStateCommandHandler(
    IStateRepository stateRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateStateCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateStateCommand, ApiResponse<StateDto>>
{
    public async Task<ApiResponse<StateDto>> Handle(UpdateStateCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(e => e.ErrorMessage));

            return ApiResponse<StateDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var state = await stateRepository.GetByIdAsync(request.StateId,cancellationToken);

        if (state is null)
        {
            return ApiResponse<StateDto>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.NotFound);
        }
        mapper.Map(request, state);
        try
        {
            await stateRepository.UpdateAsync(state, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<StateDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.Conflict);
        }

        return ApiResponse<StateDto>.SuccessResponse(mapper.Map<StateDto>(state),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.OK);
    }
}