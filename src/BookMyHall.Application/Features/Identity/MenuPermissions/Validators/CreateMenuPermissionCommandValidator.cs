using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateMenuPermissionCommandValidator
    : AbstractValidator<CreateMenuPermissionCommand>
{
    public CreateMenuPermissionCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.MenuId)
            .Required(localizer, EntityKeys.Menu);

        RuleFor(x => x.PermissionId)
            .Required(localizer, EntityKeys.Permission);
    }
}