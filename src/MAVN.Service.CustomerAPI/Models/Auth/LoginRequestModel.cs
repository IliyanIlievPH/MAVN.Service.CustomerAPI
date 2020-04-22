using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Models.Auth
{
    [PublicAPI]
    public class LoginRequestModel
    {
        [Required, DataType(DataType.EmailAddress)]
        [RegularExpression(Patterns.EmailValidationPattern)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
