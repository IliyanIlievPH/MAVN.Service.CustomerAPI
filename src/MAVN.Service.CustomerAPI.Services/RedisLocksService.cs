using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Core.Services;
using StackExchange.Redis;

namespace MAVN.Service.CustomerAPI.Services
{
    public class RedisLocksService : IDistributedLocksService
    {
        private readonly string _keyPattern;
        private readonly IDatabase _database;

        public RedisLocksService(
            [NotNull] string keyPattern,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _keyPattern = keyPattern ?? throw new ArgumentNullException(nameof(keyPattern)) ;
            _database = connectionMultiplexer.GetDatabase();
        }

        public Task<bool> TryAcquireLockAsync(string data, DateTime expiration, params object[] keys)
        {
            TimeSpan expiresIn = expiration - DateTime.UtcNow;

            return _database.LockTakeAsync(GetCacheKey(keys), data, expiresIn);
        }

        public async Task<bool> DoesLockExistAsync(params string[] keys)
        {
            var value = await _database.LockQueryAsync(GetCacheKey(keys));

            return !value.IsNullOrEmpty;
        }

        private string GetCacheKey(params object[] keys)
        {
            return string.Format(_keyPattern, keys);
        }
    }
}
