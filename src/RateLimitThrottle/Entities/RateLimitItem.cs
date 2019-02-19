namespace RateLimitThrottle.Entities
{
    public class RateLimitItem
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public int PolicyType { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
    }
}
