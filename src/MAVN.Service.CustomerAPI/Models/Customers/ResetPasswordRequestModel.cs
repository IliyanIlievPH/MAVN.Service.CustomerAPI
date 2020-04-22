namespace MAVN.Service.CustomerAPI.Models.Customers
{
    /// <summary>
    /// Contains information for the Password Reset Request
    /// </summary>
    public class ResetPasswordRequestModel
    {
        /// <summary>
        /// Email of the Customer
        /// </summary>
        public string CustomerEmail { get; set; }
        /// <summary>
        /// Password Reset Identifier used to determinate if a Password Request was made
        /// </summary>
        public string ResetIdentifier { get; set; }

        /// <summary>
        /// The new Customer Password
        /// </summary>
        public string Password { get; set; }
    }
}
