using System.Linq;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;

namespace MAVN.Service.CustomerAPI.Services
{
    public class PasswordValidator : IPasswordValidator
    {
        private readonly PasswordValidationRulesDto _validationRules;
        
        private static string _allowedCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";

        public PasswordValidator(PasswordValidationRulesDto validationRules)
        {
            _validationRules = validationRules;
        }

        public PasswordValidationRulesDto GetPasswordValidationRules()
        {
            return _validationRules;
        }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < _validationRules.MinLength)
                return false;

            if (password.Length > _validationRules.MaxLength)
                return false;

            if (!HasOnlyAcceptedSymbols(password))
                return false;

            if (!HasEnoughSpecialSymbols(password))
                return false;

            if (!HasEnoughNumbers(password))
                return false;

            if (!HasEnoughUpperCase(password))
                return false;

            if (!HasEnoughLowerCase(password))
                return false;

            if (!HasWhiteSpaceIfNotAllowed(password))
                return false;

            return true;
        }

        public string BuildValidationMessage()
        {
            return
                $"Password length should be between {_validationRules.MinLength} and {_validationRules.MaxLength} characters. " +
                $"Password should contain {_validationRules.MinLowerCase} lowercase, {_validationRules.MinUpperCase} uppercase, " +
                $"{_validationRules.MinNumbers} digits and {_validationRules.MinSpecialSymbols} special symbols. " +
                $"Allowed symbols are: {_allowedCharacters}. " +
                $"Allowed special symbols are: {_validationRules.AllowedSpecialSymbols}. " +
                $"Whitespaces are {(_validationRules.AllowWhiteSpaces ? "" : "not")} allowed";
        }

        private bool HasEnoughSpecialSymbols(string password)
        {
            return password.Count
                       (s => _validationRules.AllowedSpecialSymbols.Contains(s)) >= _validationRules.MinSpecialSymbols;
        }
        
        private bool HasWhiteSpaceIfNotAllowed(string password)
        {
            if (!_validationRules.AllowWhiteSpaces && password.Any(char.IsWhiteSpace))
                return false;

            return true;
        }

        private bool HasOnlyAcceptedSymbols(string password)
        {
            foreach (var c in password)
            {
                if (_allowedCharacters.Contains(c) || _validationRules.AllowedSpecialSymbols.Contains(c))
                    continue;

                return false;
            }

            return true;
        }

        private bool HasEnoughUpperCase(string password)
            => password.Count(char.IsUpper) >= _validationRules.MinUpperCase;

        private bool HasEnoughLowerCase(string password)
            => password.Count(char.IsLower) >= _validationRules.MinLowerCase;

        private bool HasEnoughNumbers(string password)
            => password.Count(char.IsDigit) >= _validationRules.MinNumbers;
    }
}
