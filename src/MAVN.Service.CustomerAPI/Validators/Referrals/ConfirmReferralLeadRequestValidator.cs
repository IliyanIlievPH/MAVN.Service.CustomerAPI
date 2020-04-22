using FluentValidation;
using MAVN.Service.CustomerAPI.Models.Referral;

namespace MAVN.Service.CustomerAPI.Validators.Referrals
{
    public class ConfirmReferralLeadRequestValidator : AbstractValidator<ConfirmReferralLeadRequest>
    {
        public ConfirmReferralLeadRequestValidator()
        {
            RuleFor(m => m.ConfirmationCode)
                .NotEmpty()
                .WithMessage(x => $"{nameof(x.ConfirmationCode)} is required");
        }
    }
}
