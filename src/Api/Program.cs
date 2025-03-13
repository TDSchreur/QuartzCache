using Serilog;
using TDS.QuartzCache.CertificateCache;

namespace TDS.QuartzCache.Api;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     // .MinimumLevel.Warning()
                     .MinimumLevel.Information()
                     .WriteTo.Console()
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
