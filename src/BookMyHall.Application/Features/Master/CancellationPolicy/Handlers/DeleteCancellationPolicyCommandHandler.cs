using System.Net;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteCancellationPolicyCommandHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteCancellationPolicyCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCancellationPolicyCommand request,CancellationToken cancellationToken)
    {
        var policy = await cancellationPolicyRepository.GetByIdAsync(request.CancellationPolicyId,cancellationToken);
        if (policy is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.CancellationPolicy),HttpStatusCode.NotFound);
        }

        policy.IsActive = false;
        await cancellationPolicyRepository.UpdateAsync(policy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.OK);
    }
}