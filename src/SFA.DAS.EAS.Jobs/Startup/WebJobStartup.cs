using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Jobs.StartupJobs;

namespace SFA.DAS.EAS.Jobs.Startup
{
    public static class WebJobStartup
    {
        public static IHostBuilder ConfigureDasWebJobs(this IHostBuilder builder)
        {
            builder.ConfigureWebJobs(b => b.AddAzureStorageCoreServices().AddTimers());

            //todo: is this still necessary?
#pragma warning disable 618
            //builder.ConfigureServices(s => s.AddSingleton<IWebHookProvider>(p => null));
#pragma warning restore 618

            builder.ConfigureServices(s => s.AddSingleton<CreateReadStoreDatabaseJob>());

            return builder;
        }
    }
}