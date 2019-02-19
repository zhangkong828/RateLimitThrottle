namespace RateLimitThrottle.Models
{
    public class RedisOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { set; get; }

        /// <summary>
        /// Key前缀
        /// </summary>
        public string KeyPrefix { set; get; }
    }
}
