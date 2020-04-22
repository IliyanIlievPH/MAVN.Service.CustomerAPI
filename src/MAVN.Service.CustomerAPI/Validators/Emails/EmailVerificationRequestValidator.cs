using FluentValidation;
using MAVN.Service.CustomerAPI.Models.Emails;

namespace MAVN.Service.CustomerAPI.Validators.Emails
{
    public class EmailVerificationRequestValidator : AbstractValidator<EmailVerificationRequest>
    {
        public EmailVerificationRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(m => m.VerificationCode)
                .NotEmpty()
                .WithMessage("VerificationCode is required");
        }
    }
}
