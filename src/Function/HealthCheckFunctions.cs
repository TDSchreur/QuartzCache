// using System.Net;
// using System.Net.Mime;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
// using Microsoft.Extensions.Diagnostics.HealthChecks;
//
// namespace TDS.QuartzCache.Function;
//
// public class HealthCheckFunctions(HealthCheckService healthCheck)
// {
//     [Function("Health")]
//     [OpenApiOperation(operationId: "Health", tags: ["Health"])]
//     [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(HealthReport), Description = "The OK response")]
//     [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.ServiceUnavailable, Description = "The service is unavailable")]
//     public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
//     {
//         var report = await healthCheck.CheckHealthAsync().ConfigureAwait(false);
//
//         return report.Status == HealthStatus.Unhealthy
//             ? new StatusCodeResult(StatusCodes.Status503ServiceUnavailable)
//             : new OkObjectResult(report);
//     }
// }
