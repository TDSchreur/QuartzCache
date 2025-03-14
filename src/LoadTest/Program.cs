using NBomber.CSharp;
using NBomber.Sinks.Timescale;
using ZSpitz.Util;

namespace TDS.QuartzCache.LoadTest;

public static class Program
{
    static void Main()
    {
        Uri target = new("http://localhost:5122/getvalue");
        TimescaleDbSink timescaleDbSink = new();
        var client = new HttpClient();

        var scenario = Scenario.Create("user_flow_scenario", async context =>
                               {
                                   Enumerable.Range(1, 100).ForEach(async i =>
                                   {
                                       await Step.Run("get_value", context, async () =>
                                       {
                                           var response = await client.GetAsync(target).ConfigureAwait(false);
                                           return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                                       }).ConfigureAwait(false);
                                   });

                                   var response = await client.GetAsync(target).ConfigureAwait(false);
                                   return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();

                               })
                               .WithWarmUpDuration(TimeSpan.FromSeconds(3))
                               .WithLoadSimulations(
                                   Simulation.RampingInject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)), // rump-up to rate 200
                                   Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(5)), // keep injecting with rate 200
                                   Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)) // rump-down to rate 0
                               );

        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadInfraConfig("config.json")
            .WithReportingSinks(timescaleDbSink)
            .WithTestSuite("reporting")
            .WithTestName("timescale_db_demo")
            .Run();
    }
}
