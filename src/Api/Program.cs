using Microsoft.Extensions.Caching.Memory;
using Quartz;
using Serilog;

namespace Rdw.CIP.QuartzCache.Api;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .WriteTo.Console()
                     .CreateLogger();

        try
        {
            Log.Information("Starting host");
            
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSerilog();
            builder.Services.AddMemoryCache();
            builder.Services.AddQuartz(q =>
            {
                // Just use the name of your job that you created in the Jobs folder.
                var jobKey = new JobKey(nameof(StoreCertInCacheJob));
                q.AddJob<StoreCertInCacheJob>(opts =>
                {
                    opts.DisallowConcurrentExecution(true);
                    opts.WithIdentity(jobKey);
                });
    
                q.AddTrigger(opts => opts.ForJob(jobKey)
                                         .StartNow()
                                         .WithIdentity($"{nameof(StoreCertInCacheJob)}-trigger")
                                         .WithSimpleSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(10)));
            });
            builder.Services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
                options.AwaitApplicationStarted = true;
            });
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            
            }

            app.MapGet("/getvalue", (HttpContext httpContext, IMemoryCache memoryCache) => memoryCache.TryGetValue(Constants.CacheKey, out int value) 
                ? Results.Ok((object?)value) 
                : Results.InternalServerError());

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
