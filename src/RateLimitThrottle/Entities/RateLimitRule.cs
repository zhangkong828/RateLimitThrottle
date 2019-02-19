namespace RateLimitThrottle.Entities
{
    public class RateLimitRule
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public string Period { get; set; }
        public long Limit { get; set; }
        public int PolicyId { get; set; }
    }
}
