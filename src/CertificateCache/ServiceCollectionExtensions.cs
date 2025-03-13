using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace TDS.QuartzCache.CertificateCache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCertificateCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICertificateProvider, CertificateProvider>();
        services.AddQuartz(q =>
        {
            // Just use the name of your job that you created in the Jobs folder.
            var jobKey = new JobKey(nameof(StoreCertInCacheJob));
            q.AddJob<StoreCertInCacheJob>(opts =>
            {
                opts.DisallowConcurrentExecution(true);
                opts.WithIdentity(jobKey);
            });

            q.AddTrigger(opts => opts.ForJob(jobKey)
                                     .StartNow()
                                     .WithIdentity($"{nameof(StoreCertInCacheJob)}-trigger")
                                     .WithSimpleSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(10)));
        });
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
            options.AwaitApplicationStarted = true;
        });

        return services;
    }
}
