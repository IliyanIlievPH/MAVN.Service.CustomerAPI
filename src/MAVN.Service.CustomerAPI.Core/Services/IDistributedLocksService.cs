
using System;
using System.Threading.Tasks;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IDistributedLocksService
    {
        Task<bool> TryAcquireLockAsync(string data, DateTime expiration, params object[] keys);

        Task<bool> DoesLockExistAsync(params string[] keys);
    }
}
