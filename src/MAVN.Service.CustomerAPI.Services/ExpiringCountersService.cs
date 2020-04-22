using System;
using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Services;
using StackExchange.Redis;

namespace MAVN.Service.CustomerAPI.Services
{
    public class ExpiringCountersService : IExpiringCountersService
    {
        private readonly IDatabase _database;
        
        private const string KeyPattern = "c-api:expiring-counters:{0}:{1}";

        public ExpiringCountersService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }
        
        public async Task<long> IncrementCounterAsync(TimeSpan expiryPeriod, string domain, string key)
        {
            var cacheKey = GetCacheKey(domain, key);
            
            var counter = await _database.StringIncrementAsync(cacheKey);

            if (counter == 1)
            {
                await _database.KeyExpireAsync(cacheKey, expiryPeriod);
            }

            return counter;
        }

        public Task<bool> ResetCounterAsync(string domain, string key)
        {
            var cacheKey = GetCacheKey(domain, key);

            return _database.KeyDeleteAsync(cacheKey);
        }
        
        private string GetCacheKey(params object[] keys)
        {
            return string.Format(KeyPattern, keys);
        }
    }
}
