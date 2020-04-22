using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IPasswordValidator
    {
        PasswordValidationRulesDto GetPasswordValidationRules();

        bool IsValidPassword(string password);

        string BuildValidationMessage();
    }
}
