using Microsoft.AspNetCore.Http;
using RateLimitThrottle.Models;
using System.Threading.Tasks;

namespace RateLimitThrottle.Middleware
{
    public class IPRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimitProcessor _processor;
        private readonly RateLimitOptions _options;

        public IPRateLimitMiddleware(RequestDelegate next,
             RateLimitOptions options,
            RateLimitProcessor processor
            )
        {
            _next = next;
            _options = options;
            _processor = processor;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(_options.OnGetIpAddress(httpContext)))
                await _next.Invoke(httpContext);

            var identity = SetIdentity(httpContext);
            var result = _processor.Check(identity);
            if (result)
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                await _processor.ReturnQuotaExceededResponse(httpContext);
            }

        }

        private RequestIdentity SetIdentity(HttpContext httpContext)
        {
            var clientIP = _options.OnGetIpAddress(httpContext);

            var request = httpContext.Request;
            return new RequestIdentity
            {
                RequestPath = $"{request.Scheme}://{request.Host.Value}{(request.Path.HasValue ? request.Path.Value : "")}{(request.QueryString.HasValue ? request.QueryString.Value : "")}",
                PolicyType = RateLimitPolicyType.IP,
                Path = httpContext.Request.Path.ToString().ToLowerInvariant(),
                HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
                Value = clientIP
            };
        }
    }
}
