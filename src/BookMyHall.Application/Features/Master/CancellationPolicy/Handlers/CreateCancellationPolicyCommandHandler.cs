using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateCancellationPolicyCommandHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateCancellationPolicyCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(
        CreateCancellationPolicyCommand request,
        CancellationToken cancellationToken)
    {
        var existingPolicy = await cancellationPolicyRepository.GetByPolicyNameAsync(
            request.PolicyName,
            cancellationToken);

        if (existingPolicy is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.CancellationPolicy),
                HttpStatusCode.BadRequest);
        }

        var policy = mapper.Map<CancellationPolicy>(request);

        policy.CancellationPolicyId = Guid.NewGuid();
        policy.IsActive = true;

        await cancellationPolicyRepository.AddAsync(policy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(
            policy.CancellationPolicyId,
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.CancellationPolicy),
            HttpStatusCode.Created);
    }
}