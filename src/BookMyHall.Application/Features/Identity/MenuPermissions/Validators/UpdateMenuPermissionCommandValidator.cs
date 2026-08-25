using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class UpdateMenuPermissionCommandValidator
    : AbstractValidator<UpdateMenuPermissionCommand>
{
    public UpdateMenuPermissionCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.MenuPermissionId)
            .Required(localizer, EntityKeys.MenuPermission);

        RuleFor(x => x.MenuId)
            .Required(localizer, EntityKeys.Menu);

        RuleFor(x => x.PermissionId)
            .Required(localizer, EntityKeys.Permission);
    }
}