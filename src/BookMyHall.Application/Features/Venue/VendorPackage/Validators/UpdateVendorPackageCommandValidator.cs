using FluentValidation;
namespace BookMyHall.Application.Features.Venue;
public class UpdateVendorPackageCommandValidator : AbstractValidator<UpdateVendorPackageCommand>
{
    public UpdateVendorPackageCommandValidator()
    {
        RuleFor(x => x.PackageName).NotEmpty().MaximumLength(150).WithMessage("Package name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Base price must be greater than zero.");
    }
}
