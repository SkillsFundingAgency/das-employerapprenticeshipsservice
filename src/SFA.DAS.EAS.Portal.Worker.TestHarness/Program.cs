using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Worker.TestHarness.Startup;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();

                var publishReserveFundingAddedEvent = host.Services.GetService<PublishReserveFundingAddedEvents>();
                await publishReserveFundingAddedEvent.Run();

                var publishCohortApprovalRequestedByProviderEvent = host.Services.GetService<PublishCohortApprovalRequestedByProviderEvents>();
                await publishCohortApprovalRequestedByProviderEvent.Run();

                var publishAddedPayeSchemeEvent = host.Services.GetService<PublishAddedPayeSchemeEvents>();
                await publishAddedPayeSchemeEvent.Run();

                await host.StopAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasAppConfiguration(args)
                .UseDasEnvironment()
                .ConfigureServices(s => s.AddDasNServiceBus())
                .ConfigureServices(s =>
                {
                    s.AddTransient<PublishReserveFundingAddedEvents, PublishReserveFundingAddedEvents>();
                    s.AddTransient<PublishCohortApprovalRequestedByProviderEvents, PublishCohortApprovalRequestedByProviderEvents>();
                    s.AddTransient<PublishAddedPayeSchemeEvents, PublishAddedPayeSchemeEvents>();
                });
    }
}