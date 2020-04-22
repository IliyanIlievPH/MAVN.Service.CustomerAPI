
using MAVN.Service.CustomerAPI.Core.Constants;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class RegistrationResultModel
    {
        /// <summary>Customer id</summary>
        public string CustomerId { get; set; }

        /// <summary>Error</summary>
        public CustomerError Error { get; set; }
    }
}
