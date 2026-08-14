using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class UpdateHallBlockCommandValidator: AbstractValidator<UpdateHallBlockCommand>
{
    public UpdateHallBlockCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.HallBlockId)
            .Required(localizer,EntityKeys.HallBlockId);

        RuleFor(x => x.HallId)
            .Required(localizer,EntityKeys.HallId);

        RuleFor(x => x.BlockFromDate)
            .NotEmpty()
            .WithMessage("Block from date is required.");

        RuleFor(x => x.BlockToDate)
            .NotEmpty()
            .WithMessage("Block to date is required.");

        RuleFor(x => x)
            .Must(x => x.BlockFromDate <= x.BlockToDate)
            .WithMessage("Block from date must be less than or equal to block to date.");

        RuleFor(x => x)
            .Must(x =>!x.StartTime.HasValue ||!x.EndTime.HasValue || x.StartTime.Value < x.EndTime.Value)
            .WithMessage("Start time must be earlier than end time.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason must not exceed 500 characters.");
    }
}