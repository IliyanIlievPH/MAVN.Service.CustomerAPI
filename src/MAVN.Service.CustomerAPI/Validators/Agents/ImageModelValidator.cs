using System;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Models.Agents;

namespace MAVN.Service.CustomerAPI.Validators.Agents
{
    [UsedImplicitly]
    public class ImageModelValidator : AbstractValidator<ImageModel>
    {
        private const int MaxFileSizeMb = 10;

        public ImageModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.DocumentType)
                .NotEqual(DocumentType.None)
                .WithMessage("Document type required");

            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage("Name required")
                .MaximumLength(100)
                .WithMessage("Name shouldn't be longer than 100 characters.");

            RuleFor(o => o.Content)
                .NotEmpty()
                .WithMessage("Content required")
                .Must(IsValidBase64String)
                .WithMessage("Content is not base64 encoded")
                .Must(IsFileSizeLessThanMaxAllowed)
                .WithMessage($"Image shouldn't be bigger than {MaxFileSizeMb}Mb.");
        }

        private static bool IsValidBase64String(string value)
        {
            if (value == null)
            {
                return false;
            }

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        private static bool IsFileSizeLessThanMaxAllowed(string value)
        {
            if (value == null)
            {
                return true;
            }

            var content = Convert.FromBase64String(value);

            return content.Length < MaxFileSizeMb * 1024 * 1024;
        }
    }
}
