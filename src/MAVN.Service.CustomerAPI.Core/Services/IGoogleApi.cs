using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;
using Refit;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IGoogleApi
    {
        [Get("/oauth2/v2/userinfo")]
        Task<GoogleUser> GetGoogleUser([Header("Authorization")] string authorization);
    }
}
