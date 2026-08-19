using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
using FluentValidation;
namespace BookMyHall.Application.Features.Identity;

public sealed class CreatePermissionCommandValidator: AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.PermissionName)
            .Required(localizer, EntityKeys.Permission)
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