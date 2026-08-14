using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
using BookMyHall.Application.Features.Master;

namespace BookMyHall.Application.Validations;

public sealed class GetHallCategoryByIdQueryValidator
    : AbstractValidator<GetHallCategoryByIdQuery>
{
    public GetHallCategoryByIdQueryValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallCategoryId)
            .Required(localizer,EntityKeys.HallCategoryId);
    }
}