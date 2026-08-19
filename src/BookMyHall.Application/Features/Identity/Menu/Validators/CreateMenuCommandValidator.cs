using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateMenuCommandValidator
    : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.MenuName)
            .Required(localizer, EntityKeys.Menu)
            .MaximumLengthLocalized(localizer, EntityKeys.Menu, 20);
    }
}