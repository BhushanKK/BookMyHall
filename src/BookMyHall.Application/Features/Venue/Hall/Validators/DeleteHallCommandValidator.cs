using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class DeleteHallCommandValidator: AbstractValidator<DeleteHallCommand>
{
    public DeleteHallCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallId).Required(localizer, EntityKeys.HallId);
    }
}