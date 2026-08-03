using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;
public sealed class CreateEventCategoryCommandValidator: AbstractValidator<CreateEventCategoryCommand>
{
    public CreateEventCategoryCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.EventCategoryName)
            .Required(localizer, EntityKeys.EventCategoryName)
            .MaximumLength(100);
    }
}