using FluentValidation;

namespace FieldOps.Application.Features.Branches.Commands;

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Branch name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Request.Emirate)
            .IsInEnum().WithMessage("Select a valid emirate.");

        RuleFor(x => x.Request.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(5).WithMessage("Address must be at least 5 characters.")
            .MaximumLength(300);

        RuleFor(x => x.Request.Trn)
            .Matches(@"^\d{15}$").WithMessage("TRN must be exactly 15 digits.")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Trn));

        RuleFor(x => x.Request.Iban)
            .Matches(@"^AE\d{21}$").WithMessage("UAE IBAN must start with AE followed by 21 digits.")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Iban));

        RuleFor(x => x.Request.InvoicePrefix)
            .NotEmpty().WithMessage("Invoice prefix is required.")
            .Matches(@"^[A-Z]{2,6}$").WithMessage("Invoice prefix must be 2–6 uppercase letters.");
    }
}

