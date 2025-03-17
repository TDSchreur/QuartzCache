using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// using TDS.QuartzCache.CertificateCache;

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
                                        // services.AddCertificateCache(host.Configuration);
                                        services.AddHealthChecks();
                                    })
                                    .Build();

        host.Run();
    }
}
