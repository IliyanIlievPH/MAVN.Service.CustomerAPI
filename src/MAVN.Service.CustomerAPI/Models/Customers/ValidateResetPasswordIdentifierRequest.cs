using System;
namespace MAVN.Service.CustomerAPI.Models.Customers
{
    public class ValidateResetPasswordIdentifierRequest
    {
        /// <summary>
        /// Password reset identifier
        /// </summary>
        public string ResetPasswordIdentifier { get; set; }
    }
}
