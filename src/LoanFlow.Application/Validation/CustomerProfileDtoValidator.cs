using FluentValidation;
using LoanFlow.Application.CustomerProfiles;

namespace LoanFlow.Application.Validation;

public sealed class CustomerProfileDtoValidator : AbstractValidator<CustomerProfileDto>
{
    public CustomerProfileDtoValidator()
    {
        RuleFor(model => model.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(model => model.DateOfBirth)
            .NotNull()
            .Must(BeAtLeast18)
            .WithMessage("Customer must be at least 18 years old.");

        RuleFor(model => model.NationalIdNumber)
            .NotEmpty()
            .Length(10, 17)
            .Must(value => value.All(char.IsDigit))
            .WithMessage("NID number must contain digits only.");

        RuleFor(model => model.MobileNumber)
            .NotEmpty()
            .Length(11)
            .Must(BeValidBangladeshMobileNumber)
            .WithMessage("Mobile number must start with 01 and contain 11 digits.");

        RuleFor(model => model.CurrentAddress)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(model => model.PermanentAddress)
            .MaximumLength(300);

        RuleFor(model => model.PermanentAddress)
            .NotEmpty()
            .When(model => !model.IsPermanentAddressSameAsPresentAddress)
            .WithMessage("Permanent address is required.");
    }

    private static bool BeAtLeast18(DateOnly? dateOfBirth)
    {
        if (dateOfBirth is null)
        {
            return false;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var age = today.Year - dateOfBirth.Value.Year;

        if (dateOfBirth.Value > today.AddYears(-age))
        {
            age--;
        }

        return age >= 18;
    }

    private static bool BeValidBangladeshMobileNumber(string mobileNumber)
    {
        return mobileNumber.StartsWith("01", StringComparison.Ordinal) &&
               mobileNumber.All(char.IsDigit);
    }
}
