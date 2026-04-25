using FluentValidation;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Subdomain)
            .NotEmpty().WithMessage("Workspace identifier is required.")
            .Matches(@"^[a-z0-9_]+$").WithMessage("Workspace must be lowercase letters, numbers, or underscores.");

        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
