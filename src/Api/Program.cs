using System.Globalization;
using Serilog;
using Serilog.Events;
using SerilogTracing;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Api;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
                     .WriteTo.Seq("http://192.168.178.17:5341", LogEventLevel.Verbose)
                     // .WriteTo.Async(sink => sink.Console(
                     //     LogEventLevel.Information,
                     //     "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                     //     CultureInfo.InvariantCulture,
                     //     theme: SystemConsoleTheme.Literate))
                     .CreateLogger();

        using var _ = new ActivityListenerConfiguration()
                      .Instrument.AspNetCoreRequests()
                      .TraceToSharedLogger();

        try
        {
            Log.Information("Starting host");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog();
            builder.Services.AddCertificateCache(builder.Configuration);

            var app = builder.Build();

            app.MapGet("/getvalue", (HttpContext httpContext, ICertificateProvider certificateProvider) =>
            {
                var cert = certificateProvider.GetCertificate();
                return Results.Ok(cert);
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
