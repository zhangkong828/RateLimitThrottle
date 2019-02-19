using Microsoft.Extensions.Options;
using RateLimitThrottle.Models;
using System.Collections.Generic;
using System.Linq;

namespace RateLimitThrottle.Stores.Default
{
    public class DefaultPolicyStore : IPolicyStore
    {
        private readonly RateLimitOptions _options;
        private readonly RateLimitStoreSettings _settings;
        public DefaultPolicyStore(RateLimitOptions options, IOptions<RateLimitStoreSettings> settings)
        {
            _options = options;
            _settings = settings.Value;
        }

        public List<RateLimitItemValve> GetWhiteList()
        {
            if (_settings != null && _settings.WhiteList != null)
            {
                if (_settings.WhiteList.Any(x => string.IsNullOrWhiteSpace(x.Prefix)))
                {
                    _settings.WhiteList.ForEach(x =>
                    {
                        x.Prefix = _options.RateLimitCounterPrefix;
                    });
                }
                return _settings.WhiteList;
            }
            return null;
        }

        public List<RateLimitPolicy> GetPolicys()
        {
            if (_settings != null && _settings.Policys != null)
            {
                if (_settings.Policys.Any(x => string.IsNullOrWhiteSpace(x.Prefix)))
                {
                    _settings.Policys.ForEach(x =>
                    {
                        x.Prefix = _options.RateLimitCounterPrefix;
                    });
                }
                return _settings.Policys;
            }
            return null;
        }
    }
}
