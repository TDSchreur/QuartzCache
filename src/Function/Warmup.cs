using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TDS.QuartzCache.Function;

public class Warmup(ILogger<Warmup> logger)
{
    [Function(nameof(Warmup))]
    public void Run([WarmupTrigger] object warmupContext, FunctionContext context)
    {
        logger.LogInformation("Function App instance is now warm!");
    }
}
