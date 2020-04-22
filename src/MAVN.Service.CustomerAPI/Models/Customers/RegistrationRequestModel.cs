using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Customers
{
    [PublicAPI]
    public class RegistrationRequestModel
    {
        /// <summary>
        /// Email of the customer used for login
        /// </summary>
        // Validation is moved to fluent validator
        public string Email { get; set; }

        /// <summary>
        /// Unique code which is used to identify which user referred this one to the application
        /// </summary>
        public string ReferralCode { get; set; }

        /// <summary>
        /// Password 
        /// </summary>
        // Validation is moved to fluent validator
        public string Password { get; set; }
        
        /// <summary>
        /// The customer first name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [RegularExpression(Validation.Patterns.NameValidationPattern)]
        public string FirstName { get; set; }

        /// <summary>
        /// The customer last name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [RegularExpression(Validation.Patterns.NameValidationPattern)]
        public string LastName { get; set; }

        /// <summary>
        /// Identifier of the country of nationality of the customer
        /// </summary>
        public int? CountryOfNationalityId { get; set; }
    }
}
