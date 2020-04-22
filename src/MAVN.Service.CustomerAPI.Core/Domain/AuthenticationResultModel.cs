using MAVN.Service.CustomerAPI.Core.Constants;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class AuthenticationResultModel
    {
        public CustomerError Error { get; set; }

        public string Token { get; set; }

        public string CustomerId { get; set; }

    }
}
