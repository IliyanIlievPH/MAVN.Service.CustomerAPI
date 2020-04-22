using System.Text.RegularExpressions;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Models.Referral;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Validators.Referrals
{
    [UsedImplicitly]
    public class ReferralLeadRequestModelValidator : AbstractValidator<ReferralLeadRequestModel>
    {
        private static readonly Regex NameRegex = new Regex(Patterns.NameValidationPattern);
        private static readonly Regex PhoneNumberRegex = new Regex(Patterns.PhoneValidationPattern);

        public ReferralLeadRequestModelValidator()
        {
            RuleFor(o => o.FirstName)
                .NotEmpty()
                .WithMessage("First name required.")
                .MaximumLength(100)
                .WithMessage("First name shouldn't be longer than 100 characters.")
                .Must(o => o != null && NameRegex.IsMatch(o))
                .WithMessage("First name contains illegal characters.");

            RuleFor(o => o.LastName)
                .NotEmpty()
                .WithMessage("Last name required.")
                .MaximumLength(100)
                .WithMessage("Last name shouldn't be longer than 100 characters.")
                .Must(o => o != null && NameRegex.IsMatch(o))
                .WithMessage("Last name contains illegal characters.");

            RuleFor(o => o.CountryPhoneCodeId)
                .GreaterThan(0)
                .WithMessage("Country phone code required");

            RuleFor(o => o.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number required.")
                .MaximumLength(50)
                .WithMessage("Phone number shouldn't be longer than 50 characters.")
                .Must(o => o != null && PhoneNumberRegex.IsMatch(o))
                .WithMessage("Phone number contains illegal characters.");

            RuleFor(o => o.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .Matches(Patterns.EmailValidationPattern)
                .WithMessage("Email should be a valid email address.");

            RuleFor(o => o.Note)
                .MaximumLength(2000)
                .WithMessage("Note shouldn't be longer than 2000 characters.");

            RuleFor(o => o.CampaignId)
                .NotEmpty();
        }
    }
}
