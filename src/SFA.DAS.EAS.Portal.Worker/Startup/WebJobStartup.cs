using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Worker.StartupJobs;

namespace SFA.DAS.EAS.Portal.Worker.Startup
{
    public static class WebJobStartup
    {
        public static IHostBuilder ConfigureDasWebJobs(this IHostBuilder builder)
        {
            builder.ConfigureWebJobs(b => b.AddAzureStorageCoreServices()
                .AddExecutionContextBinding()
                .AddTimers());

            builder.ConfigureServices(s => s.AddSingleton<CreateReadStoreDatabaseJob>());

            return builder;
        }
    }
}