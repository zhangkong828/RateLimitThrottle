using Microsoft.AspNetCore.Http;
using RateLimitThrottle.Extensions;
using System;

namespace RateLimitThrottle
{
    /// <summary>
    /// 限流配置
    /// </summary>
    public class RateLimitOptions
    {
        /// <summary>
        /// 是否启用限流，默认：true
        /// </summary>
        public bool EnableRateLimiting { get; set; } = true;

        /// <summary>
        /// 在发生限流限制时返回HTTP状态代码，默认：429(请求太多)
        /// </summary>
        public int HttpStatusCode { get; set; } = 429;

        /// <summary>
        /// 在发生限流限制时返回HTTP消息
        /// 如果没有指定，默认：请求过于频繁，调用次数超过最大限制!
        /// </summary>
        public string HttpResponseFomatterMessage { get; set; } = "请求过于频繁，调用次数超过最大限制!";

        /// <summary>
        /// 限流计数器前缀
        /// 用于区分不同接口站点的计数器（多个接口使用同一个库，需要唯一）
        /// </summary>
        public string RateLimitCounterPrefix { get; set; } = "ancrl";

        /// <summary>
        /// 策略缓存时间
        /// 单位秒，默认5分钟
        /// </summary>
        public int ProlicyCacheTime { get; set; } = 300;

        /// <summary>
        /// 获取Client,默认获取 User.Identity.Name
        /// </summary>
        public Func<HttpContext, string> OnGetClient = context => context.GetDefaultUserIdentity();

        /// <summary>
        /// 获取IP,默认获取 Head[X-Forwarded-For] 和 HttpContext.Connection.RemoteIpAddress
        /// </summary>
        public Func<HttpContext, string> OnGetIpAddress = context => context.GetIpAddress();

        /// <summary>
        /// 日志输出
        /// </summary>
        public Action<string> LogHandler = msg => Console.WriteLine(msg);

    }
}
