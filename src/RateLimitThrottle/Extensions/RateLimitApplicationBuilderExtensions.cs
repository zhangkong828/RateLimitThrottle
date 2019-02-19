using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimitThrottle.Cache;
using RateLimitThrottle.Counter;
using RateLimitThrottle.Middleware;
using RateLimitThrottle.Stores;
using System;

namespace RateLimitThrottle
{
    public static class RateLimitApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用IP限流策略
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIPRateLimiting(this IApplicationBuilder builder)
        {
            builder.Validate();
            builder.UseMiddleware<IPRateLimitMiddleware>();
            return builder;
        }

        /// <summary>
        /// 使用Client限流策略
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseClientRateLimiting(this IApplicationBuilder builder)
        {
            builder.Validate();
            builder.UseMiddleware<ClientRateLimitMiddleware>();
            return builder;
        }

        /// <summary>
        /// 验证服务是否注入
        /// </summary>
        /// <param name="builder"></param>
        internal static void Validate(this IApplicationBuilder builder)
        {
            var rateLimitOptions = builder.ApplicationServices.GetRequiredService<RateLimitOptions>();
            if (rateLimitOptions == null) throw new ArgumentNullException(nameof(rateLimitOptions));

            var logHandler = rateLimitOptions.LogHandler;

            var scopeFactory = builder.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                TestService(serviceProvider, typeof(IRateLimitCounterStore), logHandler, "未注册指定的计数器，可以使用AddRedisCounter扩展方法注册Redis计数器！");
                TestService(serviceProvider, typeof(ICacheManager), logHandler, "未注册指定的缓存，可以使用AddMemoryPolicyCache扩展方法注册Memory缓存！");
                TestService(serviceProvider, typeof(IPolicyStore), logHandler, "未注册指定的策略存储，可以使用AddDefaultPolicyStore或AddMySqlPolicyStore扩展方法注册文件存储或MySql存储！");
            }
        }


        internal static object TestService(IServiceProvider serviceProvider, Type service, Action<string> logHandler, string message = null, bool doThrow = true)
        {
            var appService = serviceProvider.GetService(service);

            if (appService == null)
            {
                var error = message ?? $"服务{service.FullName}未在DI容器中注册";

                logHandler(error);

                if (doThrow)
                {
                    throw new InvalidOperationException(error);
                }
            }

            return appService;
        }
    }
}
