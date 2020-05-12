using MAVN.Numerics;
using FluentValidation;
using MAVN.Service.CustomerAPI.Models.ConversionRate;

namespace MAVN.Service.CustomerAPI.Validators.ConversionRate
{
    public class PartnerConversionRateRequestValidator : BaseConversionRateRequestValidator<PartnerConversionRateRequestModel>
    {
        public PartnerConversionRateRequestValidator() : base()
        {
            RuleFor(r => r.PartnerId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Burn Rule Id is required.");
        }
    }
}
