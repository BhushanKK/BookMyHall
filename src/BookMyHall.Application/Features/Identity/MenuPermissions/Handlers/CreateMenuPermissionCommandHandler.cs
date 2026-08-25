using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateMenuPermissionCommandHandler(
    IMenuPermissionRepository menuPermissionRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateMenuPermissionCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreateMenuPermissionCommand, ApiResponse<MenuPermissionDto>>
{
    public async Task<ApiResponse<MenuPermissionDto>> Handle(
        CreateMenuPermissionCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                "|",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<MenuPermissionDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        var menuPermission = mapper.Map<MenuPermission>(request);

        try
        {
            await menuPermissionRepository.AddAsync(
                menuPermission,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<MenuPermissionDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.MenuPermission),
                HttpStatusCode.Conflict);
        }

        return ApiResponse<MenuPermissionDto>.SuccessResponse(
            mapper.Map<MenuPermissionDto>(menuPermission),
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuPermission),
            HttpStatusCode.Created);
    }
}