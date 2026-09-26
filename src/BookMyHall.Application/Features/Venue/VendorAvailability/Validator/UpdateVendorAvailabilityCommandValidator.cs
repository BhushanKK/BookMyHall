using FluentValidation;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorAvailabilityCommandValidator: AbstractValidator<UpdateVendorAvailabilityCommand>
{
    public UpdateVendorAvailabilityCommandValidator()
    {
        RuleFor(x => x.VendorAvailabilityId)
            .NotEmpty()
            .WithMessage("Vendor availability id is required.");

        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Vendor is required.");

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween((short)0, (short)6)
            .When(x => x.DayOfWeek.HasValue)
            .WithMessage("Day of week must be between 0 and 6.");

        RuleFor(x => x)
            .Must(x =>
                x.DayOfWeek.HasValue ||
                x.AvailableDate.HasValue)
            .WithMessage("Either DayOfWeek or AvailableDate is required.");

        RuleFor(x => x)
            .Must(x =>
                !(x.DayOfWeek.HasValue &&
                  x.AvailableDate.HasValue))
            .WithMessage("DayOfWeek and AvailableDate cannot both be specified.");

        RuleFor(x => x)
            .Must(x =>
                !x.StartTime.HasValue ||
                !x.EndTime.HasValue ||
                x.StartTime < x.EndTime)
            .WithMessage("Start time must be earlier than end time.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}