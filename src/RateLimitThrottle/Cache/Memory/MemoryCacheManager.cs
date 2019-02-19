using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace RateLimitThrottle.Cache.Memory
{
    public class MemoryCacheManager : ICacheManager
    {
        private MemoryCache _client;
        public MemoryCacheManager()
        {
            _client = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }

        public bool Set(string key, object data, int? expirySeconds = null)
        {
            if (expirySeconds.HasValue)
                _client.Set(key, data, TimeSpan.FromSeconds(expirySeconds.Value));
            else
                _client.Set(key, data);
            return true;
        }

        public T Get<T>(string key)
        {
            var result = default(T);
            _client.TryGetValue(key, out result);
            return result;
        }
    }
}
