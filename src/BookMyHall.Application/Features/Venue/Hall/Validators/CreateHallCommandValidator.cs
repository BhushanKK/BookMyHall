using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateHallCommandValidator
    : AbstractValidator<CreateHallCommand>
{
    public CreateHallCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.HallOwnerId)
            .Required(localizer, EntityKeys.HallOwnerId);

        RuleFor(x => x.HallCategoryId)
            .Required(localizer, EntityKeys.HallCategoryId);

        RuleFor(x => x.HallName)
            .Required(localizer, EntityKeys.HallName)
            .MaximumLength(200);

        RuleFor(x => x.AddressLine1)
            .Required(localizer, EntityKeys.AddressLine1)
            .MaximumLength(250);

        RuleFor(x => x.AreaId)
            .Required(localizer, EntityKeys.AreaId);

        RuleFor(x => x.ContactPersonName)
            .Required(localizer, EntityKeys.ContactPersonName)
            .MaximumLength(150);

        RuleFor(x => x.MobileNumber)
            .Required(localizer, EntityKeys.MobileNumber)
            .MaximumLength(15);

        RuleFor(x => x.Description)
            .MaximumLength(5000);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(250);

        RuleFor(x => x.Pincode)
            .MaximumLength(10);

        RuleFor(x => x.AlternateMobileNumber)
            .MaximumLength(15);

        RuleFor(x => x.EmailAddress)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress));

        RuleFor(x => x.EmailAddress)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress));

        RuleFor(x => x.Website)
            .MaximumLength(255);

        RuleFor(x => x.MinimumCapacity)
            .GreaterThan(0)
            .When(x => x.MinimumCapacity.HasValue);

        RuleFor(x => x.MaximumCapacity)
            .GreaterThan(0)
            .When(x => x.MaximumCapacity.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinimumCapacity.HasValue ||
                !x.MaximumCapacity.HasValue ||
                x.MinimumCapacity <= x.MaximumCapacity);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.GoogleMapLocationUrl)
            .MaximumLength(5000);
    }
}