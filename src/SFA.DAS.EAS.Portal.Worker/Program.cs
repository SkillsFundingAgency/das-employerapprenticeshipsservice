using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.DependencyResolution;
using SFA.DAS.EAS.Portal.Worker.Startup;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.EAS.Portal.Worker.StartupJobs;
using System;

namespace SFA.DAS.EAS.Portal.Worker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHost host = null;
            try
            {
                host = CreateHostBuilder(args).Build();

                await host.StartAsync();

                var jobHost = host.Services.GetService<IJobHost>();
                await jobHost.CallAsync(nameof(CreateReadStoreDatabaseJob.CreateReadStoreDatabase));

                await host.WaitForShutdownAsync();               

            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message + ex.StackTrace);
                throw;
            }
            finally
            {
                if (host != null)
                {
                    host.Dispose();
                }
            }
          
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureDasLogging() //todo: need to check logging/redis/use of localhost:6379 locally
                .UseApplicationInsights() // todo: need to add APPINSIGHTS_INSTRUMENTATIONKEY to config somewhere. where does it normally live? we could store it in table storage
                .UseDasEnvironment()
                .UseConsoleLifetime()
                .ConfigureServices(s => s.AddApplicationServices())
                .ConfigureServices(s => s.AddCosmosDatabase())
                .ConfigureServices(s => s.AddDasNServiceBus());
    }
}