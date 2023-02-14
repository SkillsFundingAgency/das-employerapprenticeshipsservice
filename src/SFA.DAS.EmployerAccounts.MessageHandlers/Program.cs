using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
             .ConfigureDasAppConfiguration(args)
             .UseConsoleLifetime()
             .ConfigureDasLogging()
             .ConfigureServices((context, services) =>  services.AddMemoryCache())
             .UseStructureMap()
             .ConfigureContainer<Registry>(IoC.Initialize);

        return builder.Build();
    }
}