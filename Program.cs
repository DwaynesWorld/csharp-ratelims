using RateLims;

var limiter = new SlidingWindowRateLimitService();
var key = "foo";

foreach (var i in Enumerable.Range(0, 100))
{
    using var lease = limiter.Acquire(key);
    if (!lease.IsAcquired)
    {
        Console.WriteLine($"Rate Limited {i}");
        await Task.Delay(1000);
        continue;
    }

    Console.WriteLine($"{i}");
}
