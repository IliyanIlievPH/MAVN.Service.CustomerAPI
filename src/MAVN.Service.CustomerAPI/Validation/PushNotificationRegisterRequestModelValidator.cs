using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Models.PushNotifications;

namespace MAVN.Service.CustomerAPI.Validation
{
    [UsedImplicitly]
    public class PushNotificationRegisterRequestModelValidator : AbstractValidator<PushNotificationRegisterRequestModel>
    {
        public PushNotificationRegisterRequestModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.PushRegistrationToken)
                .NotEmpty()
                .WithMessage("Push Registration Token cannot be empty")
                .Length(1, 255);
        }
    }
}
