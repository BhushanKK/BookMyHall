using System.Net;
using MediatR;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class DeleteHallCommandHandler(
    IHallRepository hallRepository,
    IUnitOfWork unitOfWork,
    IValidator<DeleteHallCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteHallCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteHallCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<bool>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var hall = await hallRepository.GetByIdAsync(request.HallId,cancellationToken);

        if (hall is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.Hall),HttpStatusCode.NotFound);
        }

        hall.IsActive = false;
        await hallRepository.UpdateAsync(hall,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,messageHelper.DeletedEntity(
                ResourceNames.Entities,EntityKeys.Hall),HttpStatusCode.OK);
    }
}