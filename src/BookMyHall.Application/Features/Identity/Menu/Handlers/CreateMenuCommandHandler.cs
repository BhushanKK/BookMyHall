using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateMenuCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<CreateMenuCommand, ApiResponse<MenuDto>>
{
    public async Task<ApiResponse<MenuDto>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<MenuDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        Menu? parentMenu = null;

        if (request.ParentMenuId.HasValue)
        {
            parentMenu = await menuRepository.GetByIdAsync(request.ParentMenuId.Value, cancellationToken);

            if (parentMenu is null)
            {
                return ApiResponse<MenuDto>.FailureResponse
                (
                    messageHelper.NotFoundEntity(ResourceNames.Entities, EntityKeys.Menu),
                    HttpStatusCode.NotFound
                );
            }
        }

        var menu = mapper.Map<Menu>(request);

        menu.Level = parentMenu is null
        ? (short)1
        : (short)(parentMenu.Level + 1);

        try
        {
            await menuRepository.AddAsync(menu, cancellationToken);
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

        await cacheService.RemoveByPrefixAsync(CacheKeys.Menus, cancellationToken);

        return ApiResponse<MenuDto>.SuccessResponse
        (
            mapper.Map<MenuDto>(menu),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Menu),
            HttpStatusCode.Created
        );
    }
}