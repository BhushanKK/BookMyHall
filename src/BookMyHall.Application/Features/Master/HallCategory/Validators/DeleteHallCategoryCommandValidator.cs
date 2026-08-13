using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class DeleteHallCategoryCommandValidator
    : AbstractValidator<DeleteHallCategoryCommand>
{
    public DeleteHallCategoryCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.HallCategoryId)
            .Required(
                localizer,
                EntityKeys.HallCategoryId);
    }
}