using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Models.Customers
{
    [PublicAPI]
    public class GenerateResetPasswordLinkRequestModel
    {
        /// <summary>
        /// Email of the customer used for login
        /// </summary>
        [Required, DataType(DataType.EmailAddress)]
        [RegularExpression(Patterns.EmailValidationPattern)]
        public string Email { get; set; }
    }
}
