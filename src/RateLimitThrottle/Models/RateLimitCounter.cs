using System;

namespace RateLimitThrottle.Models
{
    public class RateLimitCounter
    {
        public DateTime Timestamp { get; set; }

        public long TotalRequests { get; set; }
    }
}
