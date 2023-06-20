using SFA.DAS.EmployerAccounts.Jobs.Extensions;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;

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
            .ConfigureDasAppConfiguration()
            .ConfigureDasWebJobs()
            .ConfigureDasLogging()
            .ConfigureDasServices()
            .Build();
    }
}