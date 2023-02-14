using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;
using SFA.DAS.EmployerAccounts.Jobs.Extensions;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs;

public class Program
{
    public static async Task Main()
    {
        using (var host = CreateHost())
        {
            await SeedData(host);

            await host.RunAsync();
        }
    }

    private static async Task SeedData(IHost host)
    {
        var readStoreDatabaseJob = host.Services.GetService<CreateReadStoreDatabaseJob>();
        var seedAccountUsersJob = host.Services.GetService<SeedAccountUsersJob>();

        await readStoreDatabaseJob.Run();
        await seedAccountUsersJob.Run();
    }

    private static IHost CreateHost()
    {
        return new HostBuilder()
            .ConfigureDasWebJobs()
            .ConfigureDasLogging()
            .ConfigureDasServices()
            .UseStructureMap()
            .ConfigureContainer<Registry>(IoC.Initialize)
            .Build();
    }
}