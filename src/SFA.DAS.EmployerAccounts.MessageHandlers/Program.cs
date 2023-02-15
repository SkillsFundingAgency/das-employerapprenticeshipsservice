using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using SFA.DAS.EmployerAccounts.Startup;

namespace SFA.DAS.EmployerAccounts.MessageHandlers;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHost(args);

        var startup = host.Services.GetService<IStartup>();

        await startup.StartAsync();

        await host.RunAsync();

        await startup.StopAsync();
    }

    private static IHost CreateHost(string[] args)
    {
        var builder = new HostBuilder()
             .ConfigureDasAppConfiguration(args)
             .UseConsoleLifetime()
             .ConfigureDasLogging()
             .ConfigureServices(services => services.AddMemoryCache())
             .UseStructureMap()
             .ConfigureContainer<Registry>(IoC.Initialize);

        return builder.Build();
    }
}