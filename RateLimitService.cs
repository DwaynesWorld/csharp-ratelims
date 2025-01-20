using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace RateLims;

interface IRateLimitService
{
    RateLimitLease Acquire(string key);
}

public class SlidingWindowRateLimitService : IRateLimitService
{
    private readonly ConcurrentDictionary<string, RateLimiter> _limiters = new();
    private readonly SlidingWindowRateLimiterOptions _defaultOptions =
        new()
        {
            Window = TimeSpan.FromSeconds(1),
            SegmentsPerWindow = 10,
            AutoReplenishment = true,
            PermitLimit = 5,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        };

    public RateLimitLease Acquire(string key)
    {
        RateLimiter limiter;

        if (!_limiters.ContainsKey(key))
        {
            limiter = new SlidingWindowRateLimiter(_defaultOptions);
            _limiters.AddOrUpdate(key, limiter, (u, l) => l);
        }
        else
        {
            limiter = _limiters[key];
        }

        return limiter.AttemptAcquire(1);
    }
}
