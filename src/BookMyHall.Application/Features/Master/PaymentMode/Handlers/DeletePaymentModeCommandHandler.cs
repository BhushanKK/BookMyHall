using System.Net;

using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Master;

public sealed class DeletePaymentModeCommandHandler(IPaymentModeRepository paymentModeRepository,
    IUnitOfWork unitOfWork, IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<DeletePaymentModeCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeletePaymentModeCommand request, CancellationToken cancellationToken)
    {
        var paymentMode = await paymentModeRepository.GetByIdAsync(request.PaymentModeId, cancellationToken);

        if (paymentMode is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFound(EntityKeys.PaymentMode),
                HttpStatusCode.NotFound);
        }

        paymentMode.IsDeleted = true;
        await paymentModeRepository.UpdateAsync(paymentMode, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.PaymentMode}:{request.PaymentModeId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.PaymentModesPaged}:", cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true,
            messageHelper.DeletedEntity(ResourceNames.Entities, EntityKeys.PaymentMode), HttpStatusCode.OK);
    }
}