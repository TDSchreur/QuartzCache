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
    //// [Function(nameof(HttpCatchAll))]
    //// [OpenApiOperation(operationId: nameof(HttpCatchAll), tags: ["Tests"])]
    //// [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Text.Plain, bodyType: typeof(CertResponse), Description = "The OK response")]
    //// public static IActionResult HttpCatchAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*catch-all}")] HttpRequest req)
    //// {
    ////     return new OkObjectResult(new { message = $"Hello from catch-all route! request: {req.Path}" });
    //// }

    [Function(nameof(HttpTrekker))]
    [OpenApiOperation(operationId: nameof(HttpTrekker), tags: ["Tests"])]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(CertResponse), Description = "The OK response")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "httptrekker")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var cert = certificateProvider.GetCertificate();
        return new OkObjectResult(new CertResponse { Cert = cert });
    }

    public class CertResponse
    {
        public required string Cert { get; init; }
    }
}
