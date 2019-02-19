using System.Collections.Generic;

namespace RateLimitThrottle.Models
{
    /// <summary>
    /// 限流策略
    /// </summary>
    public class RateLimitPolicy : RateLimitItemValve
    {
        /// <summary>
        /// 限流规则
        /// </summary>
        public List<RateLimitRule> Rules { get; set; }
    }
}
