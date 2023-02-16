using SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.Startup;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

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
             .ConfigureDasServices()
             .UseStructureMap()
             .ConfigureContainer<Registry>(IoC.Initialize);

        return builder.Build();
    }
}