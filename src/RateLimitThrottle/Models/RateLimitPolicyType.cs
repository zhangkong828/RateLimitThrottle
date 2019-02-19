namespace RateLimitThrottle.Models
{
    /// <summary>
    /// 限流策略类型
    /// </summary>
    public enum RateLimitPolicyType
    {
        /// <summary>
        /// Ip
        /// </summary>
        IP = 1,
        /// <summary>
        /// Client
        /// </summary>
        Client = 2
    }
}
