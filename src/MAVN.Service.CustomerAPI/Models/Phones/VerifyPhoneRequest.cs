using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Phones
{
    /// <summary>
    /// Request model for phone verification confirmation
    /// </summary>
    [PublicAPI]
    public class VerifyPhoneRequest
    {
        /// <summary>
        /// Phone verification code
        /// </summary>
        [Required]
        public string VerificationCode { get; set; }
    }
}
