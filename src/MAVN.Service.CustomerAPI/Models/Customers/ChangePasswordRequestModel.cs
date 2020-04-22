using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Customers
{
    [PublicAPI]
    public class ChangePasswordRequestModel
    {
        /// <summary>
        /// Password 
        /// </summary>
        public string Password { get; set; }
    }
}
