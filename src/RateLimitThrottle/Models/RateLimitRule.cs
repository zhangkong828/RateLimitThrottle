using System;

namespace RateLimitThrottle.Models
{
    /// <summary>
    /// 限流规则
    /// </summary>
    public class RateLimitRule
    {
        /// <summary>
        /// HTTP verb and path 
        /// </summary>
        /// <example>
        /// get:/api/values
        /// *:/api/values
        /// *
        /// </example>
        public string Endpoint { get; set; }

        /// <summary>
        /// 限流周期： 1s, 1m, 1h
        /// </summary>
        public string Period { get; set; }

        public TimeSpan? PeriodTimespan { get; set; }

        /// <summary>
        /// 在一个定义的时间段内可以发出的请求的最大数量
        /// </summary>
        public long Limit { get; set; }
    }
}
