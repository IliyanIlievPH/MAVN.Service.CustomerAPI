using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Auth
{
    [PublicAPI]
    public class GoogleLoginRequestModel
    {
        /// <summary>
        /// Access token for Google Api which will be used to retrieve customer's email
        /// </summary>
        [Required]
        public string AccessToken { get; set; }
    }
}
