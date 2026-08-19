using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class UpdateMenuCommandValidator
    : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.MenuId)
            .Required(localizer, EntityKeys.MenuId);

        RuleFor(x => x.MenuName)
            .Required(localizer, EntityKeys.Menu)
            .MaximumLengthLocalized(localizer, EntityKeys.Menu, 20);
    }
}