using FluentValidation;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.Customers;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Validators
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequestModel>
    {
        public RegistrationRequestValidator(IPasswordValidator passwordValidator)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("The Email field is required.")
                .Matches(Patterns.EmailValidationPattern)
                .WithMessage("A valid email address is required.");
            RuleFor(x => x.Password)
                .Must(passwordValidator.IsValidPassword)
                .WithMessage(passwordValidator.BuildValidationMessage());
        }
    }
}
