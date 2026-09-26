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
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateCancellationPolicyCommandHandler(ICancellationPolicyRepository cancellationPolicyRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateCancellationPolicyCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateCancellationPolicyCommand, ApiResponse<CancellationPolicyDto>>
{
    public async Task<ApiResponse<CancellationPolicyDto>> Handle(CreateCancellationPolicyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<CancellationPolicyDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var policyName = request.PolicyName.Trim();
        var existingCancellationPolicy =await cancellationPolicyRepository.GetByNameIncludingDeletedAsync(policyName,cancellationToken);

        // Active record already exists.
        if (existingCancellationPolicy is not null && !existingCancellationPolicy.IsDeleted)
        {
            return ApiResponse<CancellationPolicyDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.Conflict);
        }

        // Restore previously soft-deleted record.
        if (existingCancellationPolicy is not null &&
            existingCancellationPolicy.IsDeleted)
        {
            existingCancellationPolicy.IsDeleted = false;
            existingCancellationPolicy.IsActive = true;
            existingCancellationPolicy.PolicyName =policyName;

            await cancellationPolicyRepository.UpdateAsync(existingCancellationPolicy,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingCancellationPolicy.CancellationPolicyId,cancellationToken);

            return ApiResponse<CancellationPolicyDto>.SuccessResponse(mapper.Map<CancellationPolicyDto>(existingCancellationPolicy),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.Created);
        }

        // Create a completely new record.
        var cancellationPolicy = mapper.Map<CancellationPolicy>(request);
        cancellationPolicy.CancellationPolicyId = Guid.NewGuid();
        cancellationPolicy.PolicyName = policyName;
        cancellationPolicy.IsActive = true;
        cancellationPolicy.IsDeleted = false;

        try
        {
            await cancellationPolicyRepository.AddAsync(cancellationPolicy,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<CancellationPolicyDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.CancellationPolicy),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(cancellationPolicy.CancellationPolicyId,cancellationToken);
        return ApiResponse<CancellationPolicyDto>.SuccessResponse(mapper.Map<CancellationPolicyDto>(cancellationPolicy),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid cancellationPolicyId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.CancellationPolicies}:{cancellationPolicyId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CancellationPoliciesPaged}:",cancellationToken);
    }
}