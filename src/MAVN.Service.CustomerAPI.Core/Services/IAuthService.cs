using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IAuthService
    {
        Task<AuthenticationResultModel> GoogleAuthenticateAsync(string accessToken);
    }
}
