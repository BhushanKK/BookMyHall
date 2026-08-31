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

public sealed class CreateAreaCommandHandler(
    IAreaRepository areaRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateAreaCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<CreateAreaCommand, ApiResponse<AreaDto>>
{
    public async Task<ApiResponse<AreaDto>> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<AreaDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var area = mapper.Map<Area>(request);
        area.AreaId = Guid.NewGuid();
        area.IsActive = true;

        try
        {
            await areaRepository.AddAsync(area, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<AreaDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Area), HttpStatusCode.Conflict);
        }
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.AreasPaged}:", cancellationToken);
        return ApiResponse<AreaDto>.SuccessResponse(
            mapper.Map<AreaDto>(area),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Area), HttpStatusCode.Created);
    }
}