using Microsoft.Extensions.Caching.Memory;
using Quartz;

namespace Rdw.CIP.QuartzCache.Api;

public class StoreCertInCacheJob(IMemoryCache memoryCache, ILogger<StoreCertInCacheJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        int random = Random.Shared.Next();
        logger.LogInformation("The random value is {random}", random);
        
        memoryCache.Set(Constants.CacheKey, random, DateTimeOffset.MaxValue);
        
        return Task.CompletedTask;
    }
}
