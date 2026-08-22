using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;
public sealed class CreateHallPricingCommandHandler(
    IHallPricingRepository hallPricingRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateHallPricingCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateHallPricingCommand, ApiResponse<HallPricingDto>>
{
    public async Task<ApiResponse<HallPricingDto>> Handle(
        CreateHallPricingCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<HallPricingDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var hallPricing = mapper.Map<HallPricing>(request);

        try
        {
            await hallPricingRepository.AddAsync(hallPricing,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallPricingDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.HallPricing),
                HttpStatusCode.Conflict
            );
        }
        
       await cacheService.RemoveByPrefixAsync($"{CacheKeys.HallPricing}:", cancellationToken);

        return ApiResponse<HallPricingDto>.SuccessResponse
        (
            mapper.Map<HallPricingDto>(hallPricing),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.HallPricing),
            HttpStatusCode.Created
        );
    }
}