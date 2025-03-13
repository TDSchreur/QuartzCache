using System.Globalization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Api;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .WriteTo.Async(sink => sink.Console(
                         LogEventLevel.Information,
                         "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                         CultureInfo.InvariantCulture,
                         theme: SystemConsoleTheme.Literate))
                     .CreateLogger();

        try
        {
            Log.Information("Starting host");

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSerilog();
            builder.Services.AddCertificateCache();

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
