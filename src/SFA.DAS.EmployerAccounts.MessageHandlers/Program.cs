using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHost(args);

        await host.RunAsync();
    }

    private static IHost CreateHost(string[] args)
    {
        var builder = new HostBuilder()
             .UseDasEnvironment()
             .ConfigureDasAppConfiguration(args)
             .UseConsoleLifetime()
             .ConfigureLogging((context, loggingBuilder) =>
             {
                 var appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                 if (!string.IsNullOrEmpty(appInsightsKey))
                 {
                     loggingBuilder.AddNLog(context.HostingEnvironment.IsDevelopment() ? "nlog.development.config" : "nlog.config");
                     loggingBuilder.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = appInsightsKey);
                 }
             }).ConfigureServices((context, services) =>
             {
                 services.AddMemoryCache();
             })
             .UseStructureMap()
             .ConfigureContainer<Registry>(IoC.Initialize);

        return builder.Build();
    }
}