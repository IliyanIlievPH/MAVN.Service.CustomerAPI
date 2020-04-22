using FluentValidation;
using MAVN.Service.CustomerAPI.Models.Referral;

namespace MAVN.Service.CustomerAPI.Validators.Referrals
{
    public class ConfirmReferralHotelRequestValidator : AbstractValidator<ConfirmReferralHotelRequest>
    {
        public ConfirmReferralHotelRequestValidator()
        {
            RuleFor(m => m.ConfirmationCode)
                .NotEmpty()
                .WithMessage(x => $"{nameof(x.ConfirmationCode)} is required");
        }
    }
}
