using CSRedis;
using RateLimitThrottle.Models;
using System;

namespace RateLimitThrottle.Counter.Redis
{
    /// <summary>
    /// Redis计数器
    /// </summary>
    public class RedisRateLimitCounterStore : IRateLimitCounterStore
    {
        private readonly CSRedisClient _client;
        private readonly RedisOptions _options;
        public RedisRateLimitCounterStore(RedisOptions options)
        {
            _options = options;
            _client = new CSRedisClient(_options.ConnectionString);
        }

        private string GetKeyPrefix(string key)
        {
            if (!string.IsNullOrWhiteSpace(_options.KeyPrefix))
                return $"{_options.KeyPrefix.TrimEnd(':')}:{key}";
            return key;
        }

        public bool Exists(string id)
        {
            var key = GetKeyPrefix(id);
            return _client.Exists(key);
        }

        public RateLimitCounter Get(string id)
        {
            var key = GetKeyPrefix(id);
            return _client.Get<RateLimitCounter>(key);
        }

        public void Remove(string id)
        {
            var key = GetKeyPrefix(id);
            _client.Del(key);
        }

        public bool Set(string id, RateLimitCounter counter, TimeSpan expirationTime)
        {
            var key = GetKeyPrefix(id);
            var expiry = (int)expirationTime.TotalSeconds;
            return _client.Set(key, counter, expiry);
        }
    }
}
