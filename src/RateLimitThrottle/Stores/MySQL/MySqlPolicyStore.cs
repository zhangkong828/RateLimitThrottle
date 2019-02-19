using Dapper;
using MySql.Data.MySqlClient;
using RateLimitThrottle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimitThrottle.Stores.MySQL
{
    public class MySqlPolicyStore : IPolicyStore
    {
        private readonly MySqlOptions _mySqlOptions;
        private readonly RateLimitOptions _options;
        public MySqlPolicyStore(RateLimitOptions options, MySqlOptions mySqlOptions)
        {
            _options = options;
            _mySqlOptions = mySqlOptions;
        }

        public List<RateLimitPolicy> GetPolicys()
        {
            var result = new List<RateLimitPolicy>();
            using (var connection = new MySqlConnection(_mySqlOptions.ConnectionString))
            {
                var sql = @"SELECT * FROM `ratelimit_item` where Type=3 and Prefix=@Prefix;
                            SELECT t2.* From `ratelimit_item` t1 inner join `ratelimit_rule` t2 on t1.Id=t2.PolicyId where t1.Type=3 and t1.Prefix=@Prefix";
                var multi = connection.QueryMultiple(sql, new { Prefix = _options.RateLimitCounterPrefix });
                var items = multi.Read<Entities.RateLimitItem>();
                var rules = multi.Read<Entities.RateLimitRule>();
                if (items != null && items.AsList().Count > 0)
                {
                    foreach (var item in items.AsList())
                    {
                        var policy = new RateLimitPolicy();
                        policy.Prefix = item.Prefix;
                        policy.PolicyType = (RateLimitPolicyType)item.PolicyType;
                        policy.Value = item.Value;
                        policy.Rules = new List<Models.RateLimitRule>();
                        rules.Where(x => x.PolicyId == item.Id).AsList().ForEach(x =>
                        {
                            policy.Rules.Add(new Models.RateLimitRule()
                            {
                                Endpoint = x.Endpoint,
                                Period = x.Period,
                                Limit = x.Limit
                            });
                        });
                        result.Add(policy);
                    }
                }
            }
            return result;
        }

        public List<RateLimitItemValve> GetWhiteList()
        {
            using (var connection = new MySqlConnection(_mySqlOptions.ConnectionString))
            {
                var sql = @"SELECT * FROM `ratelimit_item` where Type=1 and Prefix=@Prefix;";
                var items = connection.Query<Entities.RateLimitItem>(sql, new { Prefix = _options.RateLimitCounterPrefix });
                var result = new List<RateLimitItemValve>();
                items.ToList().ForEach(item =>
                {
                    result.Add(new RateLimitItemValve()
                    {
                        Prefix = item.Prefix,
                        PolicyType = (RateLimitPolicyType)item.PolicyType,
                        Value = item.Value
                    });
                });
                return result;
            }
        }
    }
}
