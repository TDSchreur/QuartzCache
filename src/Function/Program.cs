using System.Globalization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Function;

internal static class Program
{
    private static async Task<int> Main()
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
                     .WriteTo.Async(sink => sink.Console(
                         LogEventLevel.Information,
                         "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                         CultureInfo.InvariantCulture,
                         theme: SystemConsoleTheme.Literate))
                     .CreateLogger();

        try
        {
            var host = new HostBuilder().ConfigureFunctionsWebApplication()
                                        .ConfigureServices((host, services) =>
                                        {
                                            services.AddSerilog();
                                            services.AddApplicationInsightsTelemetryWorkerService();
                                            services.ConfigureFunctionsApplicationInsights();
                                            services.AddCertificateCache(host.Configuration);
                                            services.AddHealthChecks();
                                        })
                                        // Deze moet na ConfigureServices
                                        .ConfigureLogging(logging =>
                                        {
                                            //// logging.AddSerilog();

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

                                                options.MinLevel = LogLevel.Information;
                                                options.Rules.Add(new LoggerFilterRule(providerName, "Microsoft", LogLevel.Warning, null));
                                                options.Rules.Add(new LoggerFilterRule(providerName, "System", LogLevel.Warning, null));
                                                options.Rules.Add(new LoggerFilterRule(providerName, "TDS", LogLevel.Information, null));
                                            });
                                        })
                                        .Build();

            await host.RunAsync().ConfigureAwait(false);
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }
}
