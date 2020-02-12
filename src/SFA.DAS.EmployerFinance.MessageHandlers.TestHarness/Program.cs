using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.MessageHandlers.TestHarness.DependencyResolution;
using SFA.DAS.EmployerFinance.MessageHandlers.TestHarness.Scenarios;

namespace SFA.DAS.EmployerFinance.MessageHandlers.TestHarness
{
    public static class Program
    {
        public static async Task Main()
        {
            using (var container = IoC.Initialize())
            {
                var startup = container.GetInstance<NServiceBusStartup>();

                await startup.StartAsync();

                try
                {
                    await container.GetInstance<SendDraftExpireFundsCommand>().Run();
                    //await container.GetInstance<PublishCohortCreatedEvents>().Run();
                }
                finally
                {
                    await startup.StopAsync();
                }
            }
        }
    }
}
