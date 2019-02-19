namespace RateLimitThrottle.Models
{
    /// <summary>
    /// 限流名册
    /// </summary>
    public class RateLimitItemValve: RateLimitPrefix
    {
        public RateLimitPolicyType PolicyType { get; set; }
        public string Value { get; set; }
    }
}
