using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Phones
{
    /// <summary>
    /// Request model to set customer's phone number in the system
    /// </summary>
    [PublicAPI]
    public class SetCustomerPhoneRequest
    {
        /// <summary>
        /// The customer phone number.
        /// </summary>
        [Required]
        [MaxLength(15)]
        [RegularExpression(Validation.Patterns.CustomerPhoneValidationPattern)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The phone country dialing code identifier.
        /// </summary>
        [Required]
        public int CountryPhoneCodeId { get; set; }

    }
}
