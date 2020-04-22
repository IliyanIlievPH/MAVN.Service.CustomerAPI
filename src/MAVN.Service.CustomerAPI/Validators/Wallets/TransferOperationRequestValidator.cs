using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Models.Operations;

namespace MAVN.Service.CustomerAPI.Validators.Wallets
{
    [UsedImplicitly]
    public class TransferOperationRequestValidator : AbstractValidator<TransferOperationRequest>
    {
        public TransferOperationRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.ReceiverEmail)
                .NotEmpty()
                .WithMessage("Receiver email required")
                .EmailAddress()
                .WithMessage("Receiver email invalid");

            RuleFor(o => o.Amount)
                .GreaterThan(0)
                .WithMessage("Amount should be greater than 0")
                .LessThan(int.MaxValue)
                .WithMessage($"Amount should be less than {int.MaxValue}");
        }
    }
}
