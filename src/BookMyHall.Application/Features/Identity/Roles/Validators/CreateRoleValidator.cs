using FluentValidation;
using BookMyHall.Application.Features.Identity;

namespace BookMyHall.Application.Validations;

public sealed class CreateRoleCommandValidator
    : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class UpdateRoleCommandValidator
    : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role Id is required.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(100)
            .WithMessage("Role name cannot exceed 100 characters.");
    }
}
