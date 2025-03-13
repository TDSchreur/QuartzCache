using Quartz;

namespace Rdw.CIP.QuartzCache.Api;

public class StoreCertInCacheJob(ICertificateProvider certificateProvider, ILogger<StoreCertInCacheJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Refreshing the certificate");

        await certificateProvider.UpdateCertificate();
    }
}
