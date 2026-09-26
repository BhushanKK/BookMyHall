using FluentValidation;
namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorCommandValidator: AbstractValidator<UpdateVendorCommand>
{
    public UpdateVendorCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty();

        RuleFor(x => x.BusinessName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DisplayName)
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ContactPersonName)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.MobileNumber)
            .MaximumLength(20);

        RuleFor(x => x.AlternateMobileNumber)
            .MaximumLength(20);

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(500);

        RuleFor(x => x.AddressLine1)
            .MaximumLength(250);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(250);

        RuleFor(x => x.Pincode)
            .MaximumLength(10);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.EstablishedYear)
            .InclusiveBetween((short)1800, (short)2100)
            .When(x => x.EstablishedYear.HasValue);
    }
}