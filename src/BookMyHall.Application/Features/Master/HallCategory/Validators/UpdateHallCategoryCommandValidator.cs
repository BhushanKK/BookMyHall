using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class UpdateHallCategoryCommandValidator: AbstractValidator<UpdateHallCategoryCommand>
{
    public UpdateHallCategoryCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallCategoryId)
            .Required(localizer,EntityKeys.HallCategoryId);

        RuleFor(x => x.HallCategoryName)
            .Required(localizer,EntityKeys.HallCategory)
            .MaximumLengthLocalized(localizer,
                EntityKeys.HallCategory,100);
    }
}