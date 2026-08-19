using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.PermissionId)
            .Required(localizer, EntityKeys.PermissionId)
            .NotEmpty()
            .WithMessage("Permission id is required.");

        RuleFor(x => x.PermissionName)
            .Required(localizer, EntityKeys.PermissionName)
            .NotEmpty()
            .WithMessage("Permission name is required.")
            .MaximumLength(100)
            .WithMessage("Permission name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .Required(localizer, EntityKeys.Description)
            .NotEmpty()
            .WithMessage("Permission description is required.")
            .MaximumLength(500)
            .WithMessage("Permission description cannot exceed 500 characters.");
    }
}