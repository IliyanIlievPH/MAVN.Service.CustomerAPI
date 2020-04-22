using System.Text.RegularExpressions;
using FluentValidation;
using MAVN.Service.CustomerAPI.Models.Referral;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Validators.Referrals
{
    public class HotelReferralRequestModelValidator 
        : AbstractValidator<HotelReferralRequestModel>
    {
        private static readonly Regex PhoneNumberRegex = new Regex(Patterns.PhoneValidationPattern);
        
        public HotelReferralRequestModelValidator()
        {
            RuleFor(o => o.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .Matches(Patterns.EmailValidationPattern)
                .WithMessage("Email should be a valid email address.");
            
            RuleFor(o => o.CampaignId)
                .NotEmpty();
            
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

            RuleFor(o => o.CountryPhoneCodeId)
                .GreaterThan(0)
                .WithMessage("Country phone code required");
            
            RuleFor(hr => hr.FullName)
                .NotEmpty();
        }
    }
}
