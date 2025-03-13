using Microsoft.Extensions.Logging;
using Quartz;

namespace TDS.QuartzCache.CertificateCache;

public class StoreCertInCacheJob(ICertificateProvider certificateProvider, ILogger<StoreCertInCacheJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Refreshing the certificate");
        await certificateProvider.UpdateCertificate(context.CancellationToken).ConfigureAwait(false);
    }
}
