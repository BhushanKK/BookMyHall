using System.Net;

using AutoMapper;

using FluentValidation;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCancellationPolicyCommandHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateCancellationPolicyCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateCancellationPolicyCommand, ApiResponse<CancellationPolicyDto>>
{
    public async Task<ApiResponse<CancellationPolicyDto>> Handle(UpdateCancellationPolicyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<CancellationPolicyDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var policy = await cancellationPolicyRepository.GetByIdAsync(request.CancellationPolicyId, cancellationToken);
        if (policy is null)
        {
            return ApiResponse<CancellationPolicyDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.CancellationPolicy),
                HttpStatusCode.NotFound);
        }

        var existingPolicy = await cancellationPolicyRepository.GetByPolicyNameAsync(request.PolicyName, cancellationToken);

        if (existingPolicy is not null && existingPolicy.CancellationPolicyId != request.CancellationPolicyId)
        {
            return ApiResponse<CancellationPolicyDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.CancellationPolicy),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, policy);
        await cancellationPolicyRepository.UpdateAsync(policy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.CancellationPolicies}:{request.CancellationPolicyId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CancellationPoliciesPaged}:", cancellationToken);
        
        return ApiResponse<CancellationPolicyDto>.SuccessResponse(
            mapper.Map<CancellationPolicyDto>(policy),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.CancellationPolicy),
             HttpStatusCode.OK);
    }
}