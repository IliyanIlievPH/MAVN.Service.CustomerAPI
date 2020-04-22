using FluentValidation;
using MAVN.Service.CustomerAPI.Models.Customers;

namespace MAVN.Service.CustomerAPI.Validators.Customers
{
    public class ValidateResetPasswordIdentifierRequestValidator : AbstractValidator<ValidateResetPasswordIdentifierRequest>
    {
        public ValidateResetPasswordIdentifierRequestValidator()
        {
            RuleFor(x => x.ResetPasswordIdentifier)
                .NotEmpty()
                .WithMessage(x => $"{nameof(x.ResetPasswordIdentifier)} is required");
        }
    }
}
