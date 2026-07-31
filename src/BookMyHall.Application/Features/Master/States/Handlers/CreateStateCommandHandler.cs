using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
namespace BookMyHall.Application.Features.Master;

public sealed class CreateStateCommandHandler(
    IStateRepository stateRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateStateCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreateStateCommand, ApiResponse<StateDto>>
{
    public async Task<ApiResponse<StateDto>> Handle(
        CreateStateCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                " | ",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<StateDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        var state = mapper.Map<State>(request);

        try
        {
            await stateRepository.AddAsync(state, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<StateDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.State),
                HttpStatusCode.Conflict);
        }

        return ApiResponse<StateDto>.SuccessResponse
        (
            mapper.Map<StateDto>(state),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.State),
            HttpStatusCode.Created
        );
    }
}