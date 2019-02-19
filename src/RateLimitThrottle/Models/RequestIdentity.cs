namespace RateLimitThrottle.Models
{
    public class RequestIdentity
    {
        public string RequestPath { get; set; }

        public RateLimitPolicyType PolicyType { get; set; }

        public string Value { get; set; }

        public string Path { get; set; }

        public string HttpVerb { get; set; }
    }
}
