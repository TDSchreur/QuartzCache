using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Function;

public class HttpTrekker
{
    private readonly ICertificateProvider _certificateProvider;
    private readonly ILogger<HttpTrekker> _logger;

    public HttpTrekker(ICertificateProvider certificateProvider, ILogger<HttpTrekker> logger)
    {
        _certificateProvider = certificateProvider;
        _logger = logger;
    }

    [Function("HttpTrekker")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var cert = _certificateProvider.GetCertificate();
        return new OkObjectResult(cert);
    }
}
