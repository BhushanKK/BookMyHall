using MediatR;

using System.Net;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteStateCommandHandler(
    IStateRepository stateRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteStateCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteStateCommand request,CancellationToken cancellationToken)
    {
        var state = await stateRepository.GetByIdAsync(request.StateId,cancellationToken);
        if (state is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.State),HttpStatusCode.NotFound);
        }

        state.IsActive = false;
        await stateRepository.UpdateAsync(state, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.State),HttpStatusCode.OK);
    }
}