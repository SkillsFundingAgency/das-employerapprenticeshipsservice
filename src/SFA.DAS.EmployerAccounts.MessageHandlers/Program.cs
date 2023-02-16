using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using SFA.DAS.EmployerAccounts.MessageHandlers.Startup;

namespace SFA.DAS.EmployerAccounts.MessageHandlers;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHost(args);

        var startup = host.Services.GetService<NServiceBusStartup>();
        
        await startup.StartAsync();

        await host.RunAsync();

        await startup.StopAsync();
    }

    private static IHost CreateHost(string[] args)
    {
        var builder = new HostBuilder()
            .UseDasEnvironment()
            .ConfigureDasAppConfiguration(args)
            .ConfigureContainer<Registry>(IoC.Initialize)
            .UseConsoleLifetime()
            .ConfigureDasLogging()
            .ConfigureDasServices()
            .UseStructureMap();
             

        return builder.Build();
    }
}