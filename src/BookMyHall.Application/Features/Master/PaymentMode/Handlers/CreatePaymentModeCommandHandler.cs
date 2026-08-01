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

public sealed class CreatePaymentModeCommandHandler(
    IPaymentModeRepository paymentModeRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreatePaymentModeCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreatePaymentModeCommand request,CancellationToken cancellationToken)
    {
        var existingPaymentMode = await paymentModeRepository.GetByPaymentModeNameAsync(request.PaymentModeName,cancellationToken);
        if (existingPaymentMode is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.PaymentMode),
                HttpStatusCode.BadRequest);
        }

        var paymentMode = mapper.Map<PaymentMode>(request);
        paymentMode.PaymentModeId = Guid.NewGuid();
        paymentMode.IsActive = true;
        await paymentModeRepository.AddAsync(paymentMode, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<Guid>.SuccessResponse( paymentMode.PaymentModeId,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.PaymentMode),HttpStatusCode.Created);
    }
}