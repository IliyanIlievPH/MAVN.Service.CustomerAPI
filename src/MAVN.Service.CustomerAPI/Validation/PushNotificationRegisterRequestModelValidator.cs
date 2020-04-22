using FluentValidation;
using MAVN.Service.CustomerAPI.Models.PushNotifications;

namespace MAVN.Service.CustomerAPI.Validation
{
    public class PushNotificationRegisterRequestModelValidator : AbstractValidator<PushNotificationRegisterRequestModel>
    {
        public PushNotificationRegisterRequestModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.InfobipPushRegistrationId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Infobip Token cannot be empty")
                .Length(1, 255);

            When(x => string.IsNullOrEmpty(x.FirebaseToken), () =>
            {
                RuleFor(x => x.AppleToken)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Apple or Firebase Token are mandatory")
                    .Length(1, 255);
            });

            When(x => string.IsNullOrEmpty(x.AppleToken), () =>
            {
                RuleFor(x => x.FirebaseToken)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Apple or Firebase Token are mandatory")
                    .Length(1, 255);
            });

            RuleFor(x => x.AppleToken)
                .Length(0, 255);

            RuleFor(x => x.FirebaseToken)
                .Length(0, 255);
        }
    }
}
