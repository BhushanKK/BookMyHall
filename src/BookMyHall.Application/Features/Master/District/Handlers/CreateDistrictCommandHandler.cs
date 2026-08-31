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

public sealed class CreateDistrictCommandHandler(
    IDistrictRepository districtRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateDistrictCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateDistrictCommand, ApiResponse<DistrictDto>>
{
    public async Task<ApiResponse<DistrictDto>> Handle(CreateDistrictCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<DistrictDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var district = mapper.Map<District>(request);
        district.DistrictId = Guid.NewGuid();
        district.IsActive = true;

        try
        {
            await districtRepository.AddAsync(district,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<DistrictDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.CitiesPaged}:", cancellationToken);
        return ApiResponse<DistrictDto>.SuccessResponse(
            mapper.Map<DistrictDto>(district),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.Created);
    }
}