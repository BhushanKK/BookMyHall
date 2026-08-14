using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateHallCategoryCommandValidator: AbstractValidator<CreateHallCategoryCommand>
{
    public CreateHallCategoryCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallCategoryName)
            .Required(localizer,EntityKeys.HallCategory)
            .MaximumLengthLocalized(localizer,
                EntityKeys.HallCategory,100);
    }
}