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

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateMenuPermissionCommandHandler(
    IMenuPermissionRepository menuPermissionRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateMenuPermissionCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateMenuPermissionCommand, ApiResponse<MenuPermissionDto>>
{
    public async Task<ApiResponse<MenuPermissionDto>> Handle(
        UpdateMenuPermissionCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                "|",
                validationResult.Errors.Select(e => e.ErrorMessage));

            return ApiResponse<MenuPermissionDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        var menuPermission = await menuPermissionRepository.GetByIdAsync(
            request.MenuPermissionId,
            cancellationToken);

        if (menuPermission is null)
        {
            return ApiResponse<MenuPermissionDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.MenuPermission),
                HttpStatusCode.NotFound);
        }

        mapper.Map(request, menuPermission);

        try
        {
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
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.MenuPermission),
            HttpStatusCode.OK);
    }
}