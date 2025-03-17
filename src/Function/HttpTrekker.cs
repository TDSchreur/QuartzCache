// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Extensions.Logging;
// using TDS.QuartzCache.CertificateCache;
//
// namespace TDS.QuartzCache.Function;
//
// public class HttpTrekker(ICertificateProvider certificateProvider, ILogger<HttpTrekker> logger)
// {
//     [Function("HttpTrekker")]
//     public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
//     {
//         logger.LogInformation("C# HTTP trigger function processed a request.");
//
//         var cert = certificateProvider.GetCertificate();
//         return new OkObjectResult(cert);
//     }
// }
