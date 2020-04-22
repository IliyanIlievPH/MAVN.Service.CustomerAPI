using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Customers
{
    [PublicAPI]
    public class GoogleRegistrationRequestModel
    {
        /// <summary>
        /// Access token which will be used to get customer's email from google api
        /// </summary>
        [Required]
        public string AccessToken { get; set; }

        /// <summary>
        /// Unique code which is used to identify which user referred this one to the application
        /// </summary>
        public string ReferralCode { get; set; }
        
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
