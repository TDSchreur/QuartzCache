namespace Rdw.CIP.QuartzCache.Api;

public interface ICertificateProvider
{
    string GetCertificate();
    Task UpdateCertificate();
}
