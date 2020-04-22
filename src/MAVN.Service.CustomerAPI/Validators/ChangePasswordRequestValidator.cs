using FluentValidation;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.Customers;

namespace MAVN.Service.CustomerAPI.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestModel>
    {
        public ChangePasswordRequestValidator(IPasswordValidator passwordValidator)
        {
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
