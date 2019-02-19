using Microsoft.AspNetCore.Http;
using System.Linq;

namespace RateLimitThrottle.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 取得客户端IP地址
        /// </summary>
        internal static string GetIpAddress(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        /// <summary>
        /// 取得默认用户Identity值
        /// </summary>
        internal static string GetDefaultUserIdentity(this HttpContext context)
        {
            return context.User?.Identity.Name;
        }

        /// <summary>
        /// 取得Header值
        /// </summary>
        internal static string GetHeaderValue(this HttpContext context, string key)
        {
            return context.Request.Headers[key].FirstOrDefault();
        }

        /// <summary>
        /// 取得Query值
        /// </summary>
        internal static string GetQueryValue(this HttpContext context, string key)
        {
            return context.Request.Query[key].FirstOrDefault();
        }

        /// <summary>
        /// 取得RequestPath
        /// </summary>
        internal static string GetRequestPath(this HttpContext context)
        {
            return context.Request.Path.Value;
        }

        /// <summary>
        /// 取得Cookie值
        /// </summary>
        internal static string GetCookieValue(this HttpContext context, string key)
        {
            context.Request.Cookies.TryGetValue(key, out string value);
            return value;
        }

        /// <summary>
        /// 取得Form值
        /// </summary>
        internal static string GetFormValue(this HttpContext context, string key)
        {
            var value = context.Request.Form[key].FirstOrDefault();
            return value;
        }

    }
}
