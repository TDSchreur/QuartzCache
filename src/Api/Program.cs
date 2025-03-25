using System.Globalization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SerilogTracing;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Api;

internal static class Program
{
    private const int FiveMinites = 300;

    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
#pragma warning disable S1075
                     .WriteTo.Seq("http://192.168.178.17:5341", LogEventLevel.Information)
#pragma warning restore S1075
                     .WriteTo.Async(sink => sink.Console(
                         LogEventLevel.Information,
                         "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                         CultureInfo.InvariantCulture,
                         theme: SystemConsoleTheme.Literate))
                     .CreateLogger();

        using var _ = new ActivityListenerConfiguration()
                      .Instrument.AspNetCoreRequests()
                      .TraceToSharedLogger();

        try
        {
            Log.Information("Starting host");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog();
            builder.Services.AddCertificateCache(builder.Configuration);

            var app = builder.Build();

            app.MapGet("/getvalue", (HttpContext httpContext, ICertificateProvider certificateProvider) =>
            {
                var cert = certificateProvider.GetCertificate();
                return Results.Ok(cert);
            });

            app.MapPost("/delay/{delay:int}", async (int delay, HttpContext httpContext) =>
            {
                if (delay > FiveMinites)
                {
                    delay = FiveMinites;
                }

                if (delay < 1)
                {
                    delay = 1;
                }

                await Task.Delay(TimeSpan.FromSeconds(delay)).ConfigureAwait(false);

                return Results.Ok($"Delayed for {delay} seconds");
            });

            await app.RunAsync().ConfigureAwait(false);
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }
}
