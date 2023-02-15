using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness;

public static class Program
{
    public static async Task Main()
    {
        using var container = IoC.Initialize();

        var startup = container.GetInstance<NServiceBusStartup>();

        await startup.StartAsync();

        try
        {
            await container.GetInstance<PublishCreateAccountUserEvents>().Run();
            await container.GetInstance<PublishCohortCreatedEvents>().Run();
            await container.GetInstance<PublishCreatedAccountEvents>().Run();
        }
        finally
        {
            await startup.StopAsync();
        }
    }
}