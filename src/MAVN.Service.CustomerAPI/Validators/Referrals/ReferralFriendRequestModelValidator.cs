using System;
using System.Text.RegularExpressions;
using FluentValidation;
using MAVN.Service.CustomerAPI.Models.Referral;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Validators.Referrals
{
    public class ReferralFriendRequestModelValidator
        : AbstractValidator<ReferralFriendRequestModel>
    {
        private static readonly Regex FullNameRegex = new Regex(Patterns.NameValidationPattern);
        
        public ReferralFriendRequestModelValidator()
        {
            RuleFor(x => x.CampaignId)
                .Must(o => o != Guid.Empty)
                .WithMessage("Campaign id is required.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .Length(3, 200)
                .WithMessage("Full name length should be in between 3 and 200 characters long.")
                .Must(o => !string.IsNullOrEmpty(o) && FullNameRegex.IsMatch(o))
                .WithMessage("Full name field can contains only letters, periods, hyphens and single quotes.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .Matches(Patterns.EmailValidationPattern)
                .WithMessage("Email should be a valid email address.");
        }
    }
}
