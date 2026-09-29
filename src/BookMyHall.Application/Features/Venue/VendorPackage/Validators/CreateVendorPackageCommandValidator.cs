using FluentValidation;
namespace BookMyHall.Application.Features.Venue;
public class CreateVendorPackageCommandValidator : AbstractValidator<CreateVendorPackageCommand>
{
    public CreateVendorPackageCommandValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty().WithMessage("Vendor configuration reference is required.");
        RuleFor(x => x.PackageName).NotEmpty().MaximumLength(150).WithMessage("Package name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Base price must be greater than zero.");
        RuleFor(x => x.DiscountPrice)
            .LessThan(x => x.Price)
            .When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be less than the base price.");
    }
}
