using MAVN.Numerics;
using FluentValidation;
using MAVN.Service.CustomerAPI.Models.ConversionRate;

namespace MAVN.Service.CustomerAPI.Validators.ConversionRate
{
    public class BurnRuleConversionRateRequestValidator: BaseConversionRateRequestValidator<BurnRuleConversionRateRequestModel>
    {
        public BurnRuleConversionRateRequestValidator() : base()
        {
            RuleFor(r => r.BurnRuleId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Burn Rule Id is required.");
        }
    }
}
