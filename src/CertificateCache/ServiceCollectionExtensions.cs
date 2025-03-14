using System.ComponentModel.DataAnnotations;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace TDS.QuartzCache.CertificateCache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCertificateCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<CertificateCacheConfiguration>()
                .Configure(cacheConfiguration => { configuration.Bind("CertificateCache", cacheConfiguration); });

        services.AddAzureClients(builder =>
        {
            builder.UseCredential(CreateCredentials());
            var config = configuration.GetRequiredSection("CertificateCache").Get<CertificateCacheConfiguration>();
            builder.AddCertificateClient(config.KeyVaultUri);
        });

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

    private static TokenCredential CreateCredentials()
    {
        var options = new DefaultAzureCredentialOptions { ExcludeManagedIdentityCredential = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")) };

        return new DefaultAzureCredential(options);
    }
}

public class CertificateCacheConfiguration
{
    [Required] public required string CertificateName { get; init; }

    [Required] public required string KeyVaultName { get; init; }
    public Uri KeyVaultUri => new Uri($"https://{KeyVaultName}.vault.azure.net/");
}
