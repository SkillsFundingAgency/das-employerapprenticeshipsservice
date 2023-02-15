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
             .ConfigureDasAppConfiguration(args)
             .UseConsoleLifetime()
             .ConfigureDasLogging()
             .ConfigureServices((context, services) =>  services.AddMemoryCache())
             .UseStructureMap()
             .ConfigureContainer<Registry>(IoC.Initialize);

        return builder.Build();
    }
}