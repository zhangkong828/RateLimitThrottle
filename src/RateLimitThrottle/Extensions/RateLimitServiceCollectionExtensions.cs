using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimitThrottle.Cache;
using RateLimitThrottle.Cache.Memory;
using RateLimitThrottle.Counter;
using RateLimitThrottle.Counter.Memory;
using RateLimitThrottle.Counter.Redis;
using RateLimitThrottle.Models;
using RateLimitThrottle.Stores;
using RateLimitThrottle.Stores.Default;
using RateLimitThrottle.Stores.MySQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimitThrottle
{
    public static class RateLimitServiceCollectionExtensions
    {
        /// <summary>
        /// 限流中间件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, Action<RateLimitOptions> options = null)
        {
            var opts = new RateLimitOptions();
            options?.Invoke(opts);
            services.AddSingleton(opts);
            services.AddSingleton<RateLimitProcessor>();
            return services;
        }

        /// <summary>
        /// Redis计数器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisCounter(this IServiceCollection services, Action<RedisOptions> options)
        {
            var opts = new RedisOptions();
            options.Invoke(opts);
            if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            {
                throw new ArgumentException("Redis计数器，缺少Redis连接字符串");
            }
            services.AddSingleton(opts);
            services.AddSingleton<IRateLimitCounterStore, RedisRateLimitCounterStore>();
            return services;
        }

        /// <summary>
        /// Memory计数器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCounter(this IServiceCollection services)
        {
            services.AddSingleton<IRateLimitCounterStore, MemoryRateLimitCounterStore>();
            return services;
        }


        /// <summary>
        /// 内存缓存
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryPolicyCache(this IServiceCollection services)
        {
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            return services;
        }

        /// <summary>
        /// 策略 使用文件存储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultPolicyStore(this IServiceCollection services, IConfigurationSection config)
        {
            services.Configure<RateLimitStoreSettings>(config);
            services.AddSingleton<IPolicyStore, DefaultPolicyStore>();
            return services;
        }

        /// <summary>
        /// 策略 使用Mysql存储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddMySqlPolicyStore(this IServiceCollection services, Action<MySqlOptions> options)
        {
            var opts = new MySqlOptions();
            options.Invoke(opts);
            if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            {
                throw new ArgumentException("策略存储，缺少MySql连接字符串");
            }
            services.AddSingleton(opts);
            services.AddSingleton<IPolicyStore, MySqlPolicyStore>();
            return services;
        }

    }
}
