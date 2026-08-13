using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateHallPricingCommandHandler(
    IHallPricingRepository hallPricingRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateHallPricingCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateHallPricingCommand, ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        UpdateHallPricingCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<HallPricingDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var hallPricing = await hallPricingRepository.GetByIdAsync(request.HallPricingId, cancellationToken);

        if (hallPricing is null)
        {
            return ApiResponse<HallPricingDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.HallPricing),
                HttpStatusCode.NotFound
            );
        }

        mapper.Map(request, hallPricing);

        try
        {
            await hallPricingRepository.UpdateAsync(hallPricing, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallPricingDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.HallPricing),
                HttpStatusCode.Conflict
            );
        }

        return ApiResponse<HallPricingDto>.SuccessResponse
        (
            mapper.Map<HallPricingDto>(hallPricing),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.HallPricing),
            HttpStatusCode.OK
        );
    }
}