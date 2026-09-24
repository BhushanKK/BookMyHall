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

public sealed class CreatePaymentModeCommandHandler(IPaymentModeRepository paymentModeRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreatePaymentModeCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
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

       var paymentModeName = request.PaymentModeName.Trim();
        var existingPaymentMode =await paymentModeRepository.GetByNameIncludingDeletedAsync(paymentModeName,cancellationToken);

        // Active record already exists.
        if (existingPaymentMode is not null && !existingPaymentMode.IsDeleted)
        {
            return ApiResponse<PaymentModeDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.Conflict);
        }

        // Restore previously soft-deleted record.
        if (existingPaymentMode is not null &&
            existingPaymentMode.IsDeleted)
        {
            existingPaymentMode.IsDeleted = false;
            existingPaymentMode.IsActive = true;
            existingPaymentMode.PaymentModeName =paymentModeName;

            await paymentModeRepository.UpdateAsync(existingPaymentMode,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingPaymentMode.PaymentModeId,cancellationToken);

            return ApiResponse<PaymentModeDto>.SuccessResponse(mapper.Map<PaymentModeDto>(existingPaymentMode),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.Created);
        }

        // Create a completely new record.
        var paymentMode = mapper.Map<PaymentMode>(request);
        paymentMode.PaymentModeId = Guid.NewGuid();
        paymentMode.PaymentModeName = paymentModeName;
        paymentMode.IsActive = true;
        paymentMode.IsDeleted = false;

        try
        {
            await paymentModeRepository.AddAsync(paymentMode,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<PaymentModeDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                    EntityKeys.PaymentMode),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(paymentMode.PaymentModeId,cancellationToken);
        return ApiResponse<PaymentModeDto>.SuccessResponse(mapper.Map<PaymentModeDto>(paymentMode),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid paymentModeId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.PaymentMode}:{paymentModeId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.PaymentModesPaged}:",cancellationToken);
    }
}