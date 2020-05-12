using MAVN.Numerics;
using FluentValidation;
using MAVN.Service.CustomerAPI.Models.ConversionRate;

namespace MAVN.Service.CustomerAPI.Validators.ConversionRate
{
    public class BaseConversionRateRequestValidator<T> : AbstractValidator<T>
        where T : ConversionRateRequestModel
    {
        public BaseConversionRateRequestValidator()
        {
            RuleFor(r => r.Amount)
                .NotNull()
                .NotEmpty()
                .Must(r => r != null && Money18.TryParse(r, out var money18) && money18 > 0)
                .WithMessage("Amount should be valid number greater than 0.");
        }
    }
}
