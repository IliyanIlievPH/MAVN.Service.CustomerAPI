using FluentValidation;
using MAVN.Service.CustomerAPI.Models.SmartVouchers;

namespace MAVN.Service.CustomerAPI.Validators.SmartVouchers
{
    public class GetSmartVoucherCampaignsValidator : AbstractValidator<GetSmartVoucherCampaignsRequest>
    {
        public GetSmartVoucherCampaignsValidator()
        {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude value must be between -90 and 90.")
                .NotEmpty()
                .NotNull()
                .When(x => x.Longitude != null || x.RadiusInKm != null);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude value must be between -180 and 180.")
                .NotEmpty()
                .NotNull()
                .When(x => x.Latitude != null || x.RadiusInKm != null);

            RuleFor(x => x.RadiusInKm)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("RadiusInKm value must be between 1 and 100.")
                .NotEmpty()
                .NotNull()
                .When(x => x.Longitude != null || x.Latitude != null);
        }
    }
}
