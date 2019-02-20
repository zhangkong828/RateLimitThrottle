using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RateLimitThrottle.Models;

namespace RateLimitThrottle.Counter.Memory
{
    /// <summary>
    /// Memory计数器
    /// </summary>
    public class MemoryRateLimitCounterStore : IRateLimitCounterStore
    {
        private MemoryCache _client;
        public MemoryRateLimitCounterStore()
        {
            _client = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }

        public bool Exists(string id)
        {
            return _client.TryGetValue(id, out object result);
        }

        public RateLimitCounter Get(string id)
        {
            if (_client.TryGetValue(id, out object result))
                return result as RateLimitCounter;
            return null;
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }

        public bool Set(string id, RateLimitCounter counter, TimeSpan expirationTime)
        {
            _client.Set(id, counter, expirationTime);
            return true;
        }
    }
}
