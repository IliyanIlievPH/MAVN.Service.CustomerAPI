using System.Text.RegularExpressions;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Models.Agents;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Validators.Agents
{
    [UsedImplicitly]
    public class AgentRegistrationRequestModelValidator : AbstractValidator<AgentRegistrationRequestModel>
    {
        private static readonly Regex Regex = new Regex(Patterns.NameValidationPattern);
        private static readonly Regex PhoneNumberRegex = new Regex(Patterns.PhoneValidationPattern);

        public AgentRegistrationRequestModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.FirstName)
                .NotEmpty()
                .WithMessage("First name required.")
                .MaximumLength(100)
                .WithMessage("First name shouldn't be longer than 100 characters.")
                .Must(o => o != null && Regex.IsMatch(o))
                .WithMessage("First name contains illegal characters.");

            RuleFor(o => o.LastName)
                .NotEmpty()
                .WithMessage("Last name required.")
                .MaximumLength(100)
                .WithMessage("Last name shouldn't be longer than 100 characters.")
                .Must(o => o != null && Regex.IsMatch(o))
                .WithMessage("Last name contains illegal characters.");

            RuleFor(o => o.CountryOfResidenceId)
                .GreaterThan(0)
                .WithMessage("Country of residence required.");

            RuleFor(o => o.Note)
                .MaximumLength(2000)
                .WithMessage("Note shouldn't be longer than 2000 characters.");

            RuleFor(o => o.Images)
                .NotEmpty()
                .WithMessage("Images required");

            RuleForEach(o => o.Images)
                .SetValidator(new ImageModelValidator());
        }
    }
}
