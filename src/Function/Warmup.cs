using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TDS.QuartzCache.Function;

public static class Warmup
{
    [Function(nameof(Warmup))]
    public static void Run([WarmupTrigger] object warmupContext, FunctionContext context)
    {
        var logger = context.GetLogger(nameof(Warmup));
        logger.LogInformation("Function App instance is now warm!");
    }
}
