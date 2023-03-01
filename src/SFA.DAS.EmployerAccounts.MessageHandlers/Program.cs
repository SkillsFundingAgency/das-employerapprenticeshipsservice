using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;

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
        return new HostBuilder()
            .ConfigureDasAppConfiguration(args)
            .UseDasEnvironment()
            .UseConsoleLifetime()
            .ConfigureDasLogging()
            .ConfigureDasServices()
            .UseNServiceBusContainer()
            .Build();
    }
}