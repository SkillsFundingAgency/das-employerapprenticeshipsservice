using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;

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
            .ConfigureContainer<Registry>(IoC.Initialize)
            .UseConsoleLifetime()
            .ConfigureDasLogging()
            .ConfigureDasServices()
            .UseStructureMap()
            ;
        
        
        return builder.Build();
    }
}