using RateLimitThrottle.Models;
using System;

namespace RateLimitThrottle.Counter
{
    /// <summary>
    /// 计数器接口
    /// </summary>
    public interface IRateLimitCounterStore
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        bool Exists(string id);
        /// <summary>
        /// 获取
        /// </summary>
        RateLimitCounter Get(string id);
        /// <summary>
        /// 删除
        /// </summary>
        void Remove(string id);
        /// <summary>
        /// 设置
        /// </summary>
        bool Set(string id, RateLimitCounter counter, TimeSpan expirationTime);
    }
}
