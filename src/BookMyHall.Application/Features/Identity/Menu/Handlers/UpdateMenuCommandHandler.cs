using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateMenuCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateMenuCommand, ApiResponse<MenuDto>>
{
    public async Task<ApiResponse<MenuDto>> Handle(
        UpdateMenuCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));

            return ApiResponse<MenuDto>.FailureResponse
            (
                message,
                HttpStatusCode.BadRequest
            );
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null)
        {
            return ApiResponse<MenuDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Menu),
                HttpStatusCode.NotFound
            );
        }

        mapper.Map(request, menu);

        try
        {
            await menuRepository.UpdateAsync(menu,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<MenuDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Menu),
                HttpStatusCode.Conflict
            );
        }

        await cacheService.RemoveAsync($"{CacheKeys.Menus}:{request.MenuId}", cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.Menus, cancellationToken);
        
        return ApiResponse<MenuDto>.SuccessResponse
        (
            mapper.Map<MenuDto>(menu),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Menu),
            HttpStatusCode.OK
        );
    }
}