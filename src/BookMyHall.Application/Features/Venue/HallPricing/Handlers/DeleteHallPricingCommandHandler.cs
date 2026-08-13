using System.Net;
using MediatR;
using FluentValidation;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class DeleteHallPricingCommandHandler(
    IHallPricingRepository hallPricingRepository,
    IUnitOfWork unitOfWork,
    IValidator<DeleteHallPricingCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteHallPricingCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteHallPricingCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<bool>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var hallPricing = await hallPricingRepository.GetByIdAsync(request.HallPricingId,cancellationToken);

        if (hallPricing is null)
        {
            return ApiResponse<bool>.FailureResponse(messageHelper.NotFound(EntityKeys.HallPricing),HttpStatusCode.NotFound);
        }

        hallPricing.IsActive = false;
        await hallPricingRepository.UpdateAsync(hallPricing,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true,messageHelper.DeletedEntity(
                ResourceNames.Entities,EntityKeys.HallPricing),HttpStatusCode.OK);
    }
}