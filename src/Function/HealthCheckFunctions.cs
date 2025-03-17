using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TDS.QuartzCache.Function;

public class HealthCheckFunctions(HealthCheckService healthCheck)
{
    [Function("Health")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var report = await healthCheck.CheckHealthAsync().ConfigureAwait(false);

        return report.Status == HealthStatus.Unhealthy ? new StatusCodeResult(StatusCodes.Status503ServiceUnavailable) : new OkObjectResult(report);
    }
}
