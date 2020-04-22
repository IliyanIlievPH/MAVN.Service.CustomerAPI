using Falcon.Numerics;
using FluentValidation;
using MAVN.Service.CustomerAPI.Models.ConversionRate;

namespace MAVN.Service.CustomerAPI.Validators.ConversionRate
{
    public class EarnRuleConversionRateRequestValidator: BaseConversionRateRequestValidator<EarnRuleConversionRateRequestModel>
    {
        public EarnRuleConversionRateRequestValidator() : base()
        {
            RuleFor(r => r.EarnRuleId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Burn Rule Id is required.");
        }
    }
}
