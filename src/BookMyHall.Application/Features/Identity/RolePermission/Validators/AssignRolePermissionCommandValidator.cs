using FluentValidation;

using BookMyHall.Shared.Constants;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Localization;
using BookMyHall.Application.Common.Extensions;

namespace BookMyHall.Application.Validations;

public sealed class AssignRolePermissionCommandValidator: AbstractValidator<AssignRolePermissionCommand>
{
    public AssignRolePermissionCommandValidator(LocalizationService localizer)
    {
        RuleFor(x => x.RoleId)
        .Required(localizer, EntityKeys.Role)
            .NotEmpty()
            .WithMessage($"{EntityKeys.Role} is required.");

        RuleFor(x => x.PermissionId)
        .Required(localizer, EntityKeys.Permission)
            .NotEmpty()
            .WithMessage($"{EntityKeys.Permission} is required.");
    }
}