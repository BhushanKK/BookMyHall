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

public sealed class CreatePaymentModeCommandHandler(
    IPaymentModeRepository paymentModeRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreatePaymentModeCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreatePaymentModeCommand, ApiResponse<PaymentModeDto>>
{
    public async Task<ApiResponse<PaymentModeDto>> Handle(CreatePaymentModeCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<PaymentModeDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var paymentMode = mapper.Map<PaymentMode>(request);
        paymentMode.PaymentModeId = Guid.NewGuid();
        paymentMode.IsActive = true;

        try
        {
            await paymentModeRepository.AddAsync(paymentMode,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<PaymentModeDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.Conflict);
        }

        return ApiResponse<PaymentModeDto>.SuccessResponse(
            mapper.Map<PaymentModeDto>(paymentMode),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.Created);
    }
}