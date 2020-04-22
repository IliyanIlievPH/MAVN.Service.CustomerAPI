using System;
using System.Threading.Tasks;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IExpiringCountersService
    {
        Task<long> IncrementCounterAsync(TimeSpan expiryPeriod, string domain, string key);

        Task<bool> ResetCounterAsync(string domain, string key);
    }
}
