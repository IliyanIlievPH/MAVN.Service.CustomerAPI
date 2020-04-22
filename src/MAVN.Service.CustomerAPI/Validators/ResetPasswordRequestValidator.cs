using FluentValidation;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.Customers;

namespace MAVN.Service.CustomerAPI.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestModel>
    {
        public ResetPasswordRequestValidator(IPasswordValidator passwordValidator)
        {
            RuleFor(x => x.CustomerEmail)
                .NotEmpty()
                .WithMessage("The Email field is required.")
                .EmailAddress()
                .WithMessage("A valid email address is required.");
            RuleFor(x => x.ResetIdentifier)
                .NotEmpty()
                .WithMessage("Reset identifier is required");
            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage("Password is a required field")
                .NotEmpty()
                .WithMessage("Password is a required field");
            RuleFor(x => x.Password)
                .Must(passwordValidator.IsValidPassword)
                .When(x => !string.IsNullOrEmpty(x.Password))
                .WithMessage(passwordValidator.BuildValidationMessage());
        }
    }
}
