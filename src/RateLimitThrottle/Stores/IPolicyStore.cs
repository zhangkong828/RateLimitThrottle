using RateLimitThrottle.Models;
using System.Collections.Generic;

namespace RateLimitThrottle.Stores
{
    public interface IPolicyStore
    {
        /// <summary>
        /// 获取白名单
        /// </summary>
        List<RateLimitItemValve> GetWhiteList();
        /// <summary>
        /// 获取策略规则
        /// </summary>
        List<RateLimitPolicy> GetPolicys();
    }
}
