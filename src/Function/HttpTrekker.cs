using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Function;

public class HttpTrekker(ICertificateProvider certificateProvider, ILogger<HttpTrekker> logger)
{
    [Function(nameof(HttpTrekker))]
    [OpenApiOperation(operationId: nameof(HttpTrekker), tags: ["Tests"])]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(CertResponse), Description = "The OK response")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var cert = certificateProvider.GetCertificate();
        return new OkObjectResult(new CertResponse { Cert = cert });
    }

    public class CertResponse
    {
        public string Cert { get; init; }
    }
}
