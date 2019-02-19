using System;

namespace RateLimitThrottle.Extensions
{
    public static class StringExtension
    {
        public static bool ContainsIgnoreCase(this string source, string value, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return source != null && value != null && source.IndexOf(value, stringComparison) >= 0;
        }
    }
}
