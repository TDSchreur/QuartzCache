using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Function;

internal static class Program
{
    private static void Main()
    {
        var host = new HostBuilder().ConfigureFunctionsWebApplication()
                                    .ConfigureServices((host, services) =>
                                    {
                                        services.AddApplicationInsightsTelemetryWorkerService();
                                        services.ConfigureFunctionsApplicationInsights();
                                        services.AddCertificateCache(host.Configuration);
                                        services.AddHealthChecks();
                                    })
                                    // Deze moet als laatste
                                    .ConfigureLogging(logging =>
                                    {
                                        // https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows#managing-log-levels
                                        // The Application Insights SDK adds a logging filter that instructs the logger to capture only warnings and more severe logs. If you want to disable this behavior, remove the filter rule as part of service configuration:
                                        logging.Services.Configure<LoggerFilterOptions>(options =>
                                        {
                                            var providerName = typeof(Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider).FullName;
                                            var defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName == providerName); //// "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                                            if (defaultRule is not null)
                                            {
                                                options.Rules.Remove(defaultRule);
                                            }

                                            options.MinLevel = LogLevel.Warning;
                                            options.Rules.Add(new LoggerFilterRule(providerName, "Microsoft", LogLevel.Warning, null));
                                            options.Rules.Add(new LoggerFilterRule(providerName, "System", LogLevel.Warning, null));
                                            options.Rules.Add(new LoggerFilterRule(providerName, "TDS", LogLevel.Information, null));
                                        });
                                    })
                                    .Build();

        host.Run();
    }
}
