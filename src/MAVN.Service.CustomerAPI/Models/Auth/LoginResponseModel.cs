using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Auth
{
    [PublicAPI]
    public class LoginResponseModel
    {
        public string Token { get; set; }
    }
}
