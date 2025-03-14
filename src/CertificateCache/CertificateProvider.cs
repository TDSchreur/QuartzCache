using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TDS.QuartzCache.CertificateCache;

public class CertificateProvider(IMemoryCache memoryCache,
                                 IOptions<CertificateCacheConfiguration> options,
                                 CertificateClient certificateClient,
                                 ILogger<CertificateProvider> logger)
    : ICertificateProvider
{
    private readonly CertificateCacheConfiguration _config = options.Value;

    private const string CertificateCacheKey = nameof(CertificateCacheKey);

    public string GetCertificate()
    {
        var cert = memoryCache.Get<string>(CertificateCacheKey);

        if (string.IsNullOrWhiteSpace(cert))
        {
            logger.LogError("Certificate not found in cache");
            throw new CertificateNotFoundException();
        }

        logger.LogInformation("Certificate found in cache");
        return cert;
    }

    public async Task UpdateCertificate(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating certificate");

        var cert = await certificateClient.GetCertificateAsync(_config.CertificateName, cancellationToken)
                                          .ConfigureAwait(false);

        if (cert?.Value == null)
        {
            throw new CertificateNotFoundException();
        }

        memoryCache.Set(CertificateCacheKey, $"Certificate-{cert.Value.Name}-{Random.Shared.Next(100, 999)}", DateTimeOffset.Now.AddMinutes(10));
    }
}
