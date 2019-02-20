using Microsoft.AspNetCore.Http;
using RateLimitThrottle.Cache;
using RateLimitThrottle.Counter;
using RateLimitThrottle.Extensions;
using RateLimitThrottle.Models;
using RateLimitThrottle.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimitThrottle
{
    public class RateLimitProcessor
    {
        private static readonly object _processLocker = new object();

        private readonly RateLimitOptions _options;
        private readonly ICacheManager _cacheManager;
        private readonly IPolicyStore _policyStore;
        private readonly IRateLimitCounterStore _counterStore;
        public RateLimitProcessor(RateLimitOptions options, ICacheManager cacheManager, IPolicyStore policyStore, IRateLimitCounterStore counterStore)
        {
            _options = options;
            _cacheManager = cacheManager;
            _policyStore = policyStore;
            _counterStore = counterStore;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public bool Check(RequestIdentity identity)
        {
            //白名单
            if (IsWhitelisted(identity))
            {
                return true;
            }

            //规则
            var rules = GetMatchingRules(identity);
            foreach (var rule in rules)
            {
                if (rule.Limit > 0)
                {
                    //处理请求数
                    var counter = ProcessRequest(identity, rule);

                    //过期
                    var expiry = counter.Timestamp + rule.PeriodTimespan.Value;
                    if (expiry < DateTime.Now)
                    {
                        continue;
                    }

                    //超过最大数量
                    if (counter.TotalRequests > rule.Limit)
                    {
                        LogBlockedRequest(identity, counter, rule);
                        return false;
                    }
                }
                else
                {
                    var counter = ProcessRequest(identity, rule);

                    LogBlockedRequest(identity, counter, rule);
                    return false;
                }
            }

            //没有匹配到规则 默认放行
            return true;
        }

        /// <summary>
        /// 是否在白名单
        /// </summary>
        /// <param name="requestIdentity"></param>
        /// <returns></returns>
        private bool IsWhitelisted(RequestIdentity requestIdentity)
        {
            var whiteList = _cacheManager.Get<List<RateLimitItemValve>>(RateLimitCacheKey.WhiteListKey);
            if (whiteList == null)
            {
                whiteList = _policyStore.GetWhiteList();
                _cacheManager.Set(RateLimitCacheKey.WhiteListKey, whiteList, _options.ProlicyCacheTime);
            }
            if (whiteList != null && whiteList.Count > 0)
            {
                var result = whiteList.FirstOrDefault(x => x.PolicyType == requestIdentity.PolicyType && x.Value == requestIdentity.Value);
                return result != null;
            }
            return false;
        }

        /// <summary>
        /// 获取匹配的规则
        /// </summary>
        /// <param name="requestIdentity"></param>
        /// <returns></returns>
        private List<RateLimitRule> GetMatchingRules(RequestIdentity requestIdentity)
        {
            var policyList = _cacheManager.Get<List<RateLimitPolicy>>(RateLimitCacheKey.PolicysKey);
            if (policyList == null)
            {
                policyList = _policyStore.GetPolicys();
                _cacheManager.Set(RateLimitCacheKey.PolicysKey, policyList, _options.ProlicyCacheTime);
            }
            var limits = new List<RateLimitRule>();
            var policy = policyList.FirstOrDefault(x => x.PolicyType == requestIdentity.PolicyType && x.Value == requestIdentity.Value);
            if (policy != null && policy.Rules != null && policy.Rules.Count > 0)
            {
                // 搜索类似规则： "*" 和 "*:/matching_path"
                var pathLimits = policy.Rules.Where(x => $"*:{requestIdentity.Path}".ContainsIgnoreCase(x.Endpoint)).AsEnumerable();
                limits.AddRange(pathLimits);

                // 搜索类似规则： "matching_verb:/matching_path"
                var verbLimits = policy.Rules.Where(x => $"{requestIdentity.HttpVerb}:{requestIdentity.Path}".ContainsIgnoreCase(x.Endpoint)).AsEnumerable();
                limits.AddRange(verbLimits);
            }

            //获取每个周期的最小限制，如
            //规则1 * 20/1m
            //规则2 get:/api/values  10/1m
            //当前请求为get:/api/values,同时满足规则1和2，在相同周期1m内，取次数最小的10次
            limits = limits.GroupBy(x => x.Period).Select(x => x.OrderBy(o => o.Limit)).Select(x => x.First()).ToList();

            foreach (var item in limits)
            {
                item.PeriodTimespan = ConvertToTimeSpan(item.Period);
            }

            limits = limits.OrderBy(x => x.PeriodTimespan).ToList();
            return limits;
        }

        private TimeSpan ConvertToTimeSpan(string timeSpan)
        {
            var l = timeSpan.Length - 1;
            var value = timeSpan.Substring(0, l);
            var type = timeSpan.Substring(l, 1);

            switch (type)
            {
                case "d": return TimeSpan.FromDays(double.Parse(value));
                case "h": return TimeSpan.FromHours(double.Parse(value));
                case "m": return TimeSpan.FromMinutes(double.Parse(value));
                case "s": return TimeSpan.FromSeconds(double.Parse(value));
                default: throw new FormatException($"{timeSpan} 转换失败, 未知类型 {type}");
            }
        }

        /// <summary>
        /// 处理请求数
        /// </summary>
        /// <param name="requestIdentity"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private RateLimitCounter ProcessRequest(RequestIdentity requestIdentity, RateLimitRule rule)
        {
            var counter = new RateLimitCounter
            {
                Timestamp = DateTime.Now,
                TotalRequests = 1
            };

            var counterId = ComputeCounterKey(requestIdentity, rule);
            lock (_processLocker)
            {
                var entry = _counterStore.Get(counterId);
                if (entry != null)
                {
                    // 没有过期，请求数+1
                    if (entry.Timestamp + rule.PeriodTimespan.Value >= DateTime.Now)
                    {
                        var totalRequests = entry.TotalRequests + 1;
                        counter = new RateLimitCounter
                        {
                            Timestamp = entry.Timestamp,
                            TotalRequests = totalRequests
                        };
                    }
                }
                _counterStore.Set(counterId, counter, rule.PeriodTimespan.Value);
            }
            return counter;
        }

        private string ComputeCounterKey(RequestIdentity requestIdentity, RateLimitRule rule)
        {
            var key = $"{_options.RateLimitCounterPrefix}_{requestIdentity.Value}_{rule.Period}";

            key += $"_{requestIdentity.HttpVerb}_{requestIdentity.Path}";

            return key;
            //加密key
            //var idBytes = Encoding.UTF8.GetBytes(key);
            //byte[] hashBytes;
            //using (var algorithm = System.Security.Cryptography.SHA1.Create())
            //{
            //    hashBytes = algorithm.ComputeHash(idBytes);
            //}
            //return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="requestIdentity"></param>
        /// <param name="counter"></param>
        /// <param name="rule"></param>
        public void LogBlockedRequest(RequestIdentity requestIdentity, RateLimitCounter counter, RateLimitRule rule)
        {
            var log = $"{requestIdentity.RequestPath}\r\n已限制来自[{requestIdentity.PolicyType.ToString()}]{requestIdentity.Value}的请求  {requestIdentity.HttpVerb}:{requestIdentity.Path}\r\n匹配规则: {rule.Endpoint}，{rule.Limit}/{rule.Period}\r\n计数器: [{ComputeCounterKey(requestIdentity, rule)}] {counter.TotalRequests}，{counter.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}";
            _options.LogHandler.Invoke(log);
        }

        /// <summary>
        /// 处理返回值
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task ReturnQuotaExceededResponse(HttpContext httpContext)
        {
            var message = _options.HttpResponseFomatterMessage;

            httpContext.Response.StatusCode = _options.HttpStatusCode;
            httpContext.Response.ContentType = "text/plain;charset=utf-8";
            return httpContext.Response.WriteAsync(message);
        }
    }
}
