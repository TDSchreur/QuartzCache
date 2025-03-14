using Azure;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace TDS.QuartzCache.CertificateCache;

public class CertificateProvider : ICertificateProvider
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IMemoryCache _memoryCache;
    private readonly CertificateClient _certificateClient;
    private readonly ILogger<CertificateProvider> _logger;

    private const string CertificateName = "integratieplatform";

    public CertificateProvider(IMemoryCache memoryCache,
                               CertificateClient certificateClient,
                               ILogger<CertificateProvider> logger)
    {
        this._memoryCache = memoryCache;
        _certificateClient = certificateClient;
        this._logger = logger;
    }

    public string GetCertificate()
    {
        var cert = _memoryCache.Get<string>(Constants.CacheKey);
        if (!string.IsNullOrWhiteSpace(cert))
        {
            _logger.LogInformation("Certificate found in cache");
            return cert;
        }

        _logger.LogError("Certificate not found in cache");
        throw new CertificateNotFoundException();
    }

    public async Task UpdateCertificate(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating certificate");
        try
        {
            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);

            var cert = await _certificateClient.GetCertificateAsync(CertificateName, cancellationToken)
                                               .ConfigureAwait(false);

            if (cert?.Value == null)
            {
                throw new CertificateNotFoundException();
            }

            _memoryCache.Set(
                Constants.CacheKey,
                $"Certificate-{cert.Value.Name}-{Random.Shared.Next(100, 999)}",
                absoluteExpiration: DateTimeOffset.UtcNow.AddSeconds(10));
        }
        finally
        {
            _lock.Release();
        }
    }
}
