namespace TDS.QuartzCache.CertificateCache;

public interface ICertificateProvider
{
    string GetCertificate();
    Task UpdateCertificate(CancellationToken cancellationToken = default);
}
