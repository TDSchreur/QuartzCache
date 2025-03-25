using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace TDS.QuartzCache.Function;

public static class HttpCatchAll
{
    [Function(nameof(HttpCatchAll))]
    public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*catch-all}")] HttpRequestData req)
    {
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString($"Hello from catch-all route! request: {req.Url}");
        return response;
    }
}
