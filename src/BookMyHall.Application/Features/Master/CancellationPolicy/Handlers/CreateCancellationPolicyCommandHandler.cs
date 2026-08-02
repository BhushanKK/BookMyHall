using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateCancellationPolicyCommandHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateCancellationPolicyCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreateCancellationPolicyCommand, ApiResponse<CancellationPolicyDto>>
{
    public async Task<ApiResponse<CancellationPolicyDto>> Handle(CreateCancellationPolicyCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<CancellationPolicyDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var policy = mapper.Map<CancellationPolicy>(request);
        policy.CancellationPolicyId = Guid.NewGuid();
        policy.IsActive = true;

        try
        {
            await cancellationPolicyRepository.AddAsync(policy,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<CancellationPolicyDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.Conflict);
        }

        return ApiResponse<CancellationPolicyDto>.SuccessResponse(
            mapper.Map<CancellationPolicyDto>(policy),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.Created);
    }
}