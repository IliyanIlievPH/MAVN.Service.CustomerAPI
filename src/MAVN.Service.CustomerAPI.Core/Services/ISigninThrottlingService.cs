using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface ISigninThrottlingService
    {
        Task<FailedSigninResultModel> RegisterFailedSigninAsync(string email);

        Task FlushFailuresAsync(string email);

        Task<SigninLockStatusResultModel> IsSigninLockedAsync(string email);
    }
}
