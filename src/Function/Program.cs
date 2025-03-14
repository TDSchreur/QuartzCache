using Microsoft.Extensions.Hosting;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Function;

internal static class Program
{
    private static void Main()
    {
        var host = new HostBuilder().ConfigureFunctionsWebApplication()
                                    .ConfigureServices((host, services) =>
                                    {
                                        services.AddCertificateCache(host.Configuration);
                                    })
                                    .Build();

        host.Run();
    }
}
