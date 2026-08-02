using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdatePaymentModeCommandHandler(
    IPaymentModeRepository paymentModeRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdatePaymentModeCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdatePaymentModeCommand, ApiResponse<PaymentModeDto>>
{
    public async Task<ApiResponse<PaymentModeDto>> Handle(UpdatePaymentModeCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<PaymentModeDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var paymentMode = await paymentModeRepository.GetByIdAsync(request.PaymentModeId,cancellationToken);

        if (paymentMode is null)
        {
            return ApiResponse<PaymentModeDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.PaymentMode),
                HttpStatusCode.NotFound);
        }

        var existingPaymentMode = await paymentModeRepository.GetByPaymentModeNameAsync(request.PaymentModeName,cancellationToken);
        if (existingPaymentMode is not null && existingPaymentMode.PaymentModeId != request.PaymentModeId)
        {
            return ApiResponse<PaymentModeDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.PaymentMode),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, paymentMode);
        await paymentModeRepository.UpdateAsync(paymentMode,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentModeDto>.SuccessResponse(
            mapper.Map<PaymentModeDto>(paymentMode),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.OK);
    }
}