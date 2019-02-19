namespace RateLimitThrottle.Cache
{
    public interface ICacheManager
    {
        /// <summary>
        /// 获取
        /// </summary>
        T Get<T>(string key);
        /// <summary>
        /// 设置
        /// </summary>
        bool Set(string key, object data, int? expirySeconds = null);
    }
}
