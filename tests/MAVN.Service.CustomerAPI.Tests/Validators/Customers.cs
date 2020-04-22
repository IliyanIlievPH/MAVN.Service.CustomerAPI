using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Models.Customers;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.CustomerAPI.Validators;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Validators
{
    public class CustomersValidatorsTests
    {
        [Fact]
        public void RegistrationRequestIsMade_EverythingValid_ValidationShouldSucceed()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new RegistrationRequestModel
            {
                Email = "valid@mail.bg",
                Password = "passwordMock1@"
            };

            var validator = new RegistrationRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void RegistrationRequestIsMade_InvalidEmail_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new RegistrationRequestModel
            {
                Email = "invalid.email",
                Password = "passwordMock1@"
            };

            var validator = new RegistrationRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("short")]
        [InlineData("tolooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("WithoutNumber@")]
        [InlineData("WithoutSpecialSymbol1")]
        [InlineData("withoutuppercase1@")]
        [InlineData("WITHOUTLOWERCASE1@")]
        [InlineData("WithNotAllowedSymbol1<")]
        [InlineData("WithNotAllowedWhitespace1@")]
        [InlineData(null)]
        public void RegistrationRequestIsMade_InvalidPassword_ValidationShouldFail(string inputPassword)
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new RegistrationRequestModel
            {
                Email = "valid@mail.bg",
                Password = inputPassword
            };

            var validator = new RegistrationRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        private PasswordValidationRulesDto CreateDefaultValidationRules()
        {
            return new PasswordValidationRulesDto
            {
                AllowWhiteSpaces = false,
                MinLength = 6,
                MinUpperCase = 1,
                MinLowerCase = 1,
                MaxLength = 20,
                MinNumbers = 1,
                AllowedSpecialSymbols = "@#$%&",
                MinSpecialSymbols = 1
            };
        }

        [Fact]
        public void ChangePasswordRequestIsMade_EverythingValid_ValidationShouldSucceed()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new ChangePasswordRequestModel
            {
                Password = "passwordMock1@"
            };

            var validator = new ChangePasswordRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("short")]
        [InlineData("tolooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("WithoutNumber@")]
        [InlineData("WithoutSpecialSymbol1")]
        [InlineData("withoutuppercase1@")]
        [InlineData("WITHOUTLOWERCASE1@")]
        [InlineData("WithNotAllowedSymbol1<")]
        [InlineData("WithNotAllowedWhitespace1@")]
        [InlineData(null)]
        public void ChangePasswordRequestIsMade_InvalidPassword_ValidationShouldFail(string inputPassword)
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new ChangePasswordRequestModel()
            {
                Password = inputPassword
            };

            var validator = new ChangePasswordRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ResetPasswordRequestIsMade_EverythingValid_ValidationShouldSucceed()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new ResetPasswordRequestModel()
            {
                CustomerEmail = "valid@mail.bg",
                Password = "passwordMock1@",
                ResetIdentifier = "resetIdentifier"
            };

            var validator = new ResetPasswordRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ResetPasswordRequestIsMade_InvalidEmail_ValidationShouldFail()
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new ResetPasswordRequestModel()
            {
                CustomerEmail = "invalid.email",
                Password = "passwordMock1@"
            };

            var validator = new ResetPasswordRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("short")]
        [InlineData("tolooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("WithoutNumber@")]
        [InlineData("WithoutSpecialSymbol1")]
        [InlineData("withoutuppercase1@")]
        [InlineData("WITHOUTLOWERCASE1@")]
        [InlineData("WithNotAllowedSymbol1<")]
        [InlineData("WithNotAllowedWhitespace1@")]
        [InlineData(null)]
        public void ResetPasswordRequestIsMade_InvalidPassword_ValidationShouldFail(string inputPassword)
        {
            var validatorService = new PasswordValidator(CreateDefaultValidationRules());
            var request = new ResetPasswordRequestModel()
            {
                Password = inputPassword
            };

            var validator = new ResetPasswordRequestValidator(validatorService);
            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }
    }
}
