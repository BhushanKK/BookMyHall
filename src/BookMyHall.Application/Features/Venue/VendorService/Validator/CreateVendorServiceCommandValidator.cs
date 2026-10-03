using FluentValidation;
namespace BookMyHall.Application.Features.Venue;
public sealed class CreateVendorServiceCommandValidator: AbstractValidator<CreateVendorServiceCommand>
{
    public CreateVendorServiceCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Vendor is required.");

        RuleFor(x => x.VendorSubCategoryId)
            .NotEmpty()
            .WithMessage("Vendor sub category is required.");

        RuleFor(x => x.VendorCategoryId).NotEmpty().WithMessage("Vendor category is required.");

        RuleFor(x => x.ServiceName)
            .NotEmpty()
            .WithMessage("Service name is required.")
            .MaximumLength(200)
            .WithMessage(
                "Service name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage(
                "Description cannot exceed 1000 characters.");

        RuleFor(x => x.PricingType)
            .NotEmpty()
            .Must(value => value is "StartingFrom" or "Fixed" or "Range")
            .WithMessage("Pricing type must be StartingFrom, Fixed, or Range.");

        RuleFor(x => x.UnitName)
            .MaximumLength(100)
            .When(x => x.UnitName is not null);

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.BasePrice.HasValue);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinPrice.HasValue ||
                !x.MaxPrice.HasValue ||
                x.MinPrice <= x.MaxPrice)
            .WithMessage(
                "Minimum price cannot be greater than maximum price.");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimumQuantity.HasValue);

        RuleFor(x => x.MaximumQuantity)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaximumQuantity.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinimumQuantity.HasValue ||
                !x.MaximumQuantity.HasValue ||
                x.MinimumQuantity <= x.MaximumQuantity)
            .WithMessage(
                "Minimum quantity cannot be greater than maximum quantity.");
    }
}
