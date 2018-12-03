using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness
{
    public static class Program
    {
        public static async Task Main()
        {
            using (var container = IoC.Initialize())
            {
                var startup = container.GetInstance<NServiceBusStartup>();

                await startup.StartAsync();

                var scenario = container.GetInstance<PublishUserRolesUpdatedAndDeletedEvents>();

                try
                {
                    await scenario.Run();
                }
                finally
                {
                    await startup.StopAsync();
                }
            }
        }
    }
}
