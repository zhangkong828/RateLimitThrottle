using System.Collections.Generic;

namespace RateLimitThrottle.Models
{
    /// <summary>
    /// 限流配置
    /// </summary>
    public class RateLimitStoreSettings
    {
        /// <summary>
        /// 白名单
        /// </summary>
        public List<RateLimitItemValve> WhiteList { get; set; }

        /// <summary>
        /// 策略
        /// </summary>
        public List<RateLimitPolicy> Policys { get; set; }
    }
}
