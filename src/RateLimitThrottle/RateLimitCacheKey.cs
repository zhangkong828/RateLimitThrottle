using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimitThrottle
{
    /// <summary>
    /// 缓存key
    /// </summary>
    public class RateLimitCacheKey
    {
        public static readonly string PolicysKey = "PolicysCacheKey";
        public static readonly string WhiteListKey = "WhiteListCacheKey";
    }
}
