using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace TDS.QuartzCache.Function;

public static class HttpCatchAll
{
    [Function(nameof(HttpCatchAll))]
    public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*catch-all}")] HttpRequest req)
    {
        return new OkObjectResult(new { message = $"Hello from catch-all route! request: {req.Path}" });
    }
}
