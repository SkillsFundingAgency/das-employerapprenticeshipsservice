using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;

namespace SFA.DAS.EmployerAccounts.Jobs.Extensions;

public static class HostExtensions
{
    public static IHostBuilder ConfigureDasLogging(this IHostBuilder builder)
    {
        builder.ConfigureLogging((context, builder) =>
        {
            builder.AddNLog();
            builder.AddConsole();
        });

        return builder;
    }

    public static IHostBuilder ConfigureDasWebJobs(this IHostBuilder builder)
    {
        builder.ConfigureWebJobs(config =>
        {
            config.AddTimers();
            config.AddAzureStorageCoreServices();
        });

        return builder;
    }

    public static IHostBuilder ConfigureDasAppConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddAzureTableStorage(ConfigurationKeys.EmployerAccounts)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
        });
    }

    public static IHostBuilder ConfigureDasServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
            {
                services.AddNServiceBus();
                services.AddScoped<CreateReadStoreDatabaseJob>();
                services.AddScoped<SeedAccountUsersJob>();
                services.AddTransient<IRunOnceJobsService, RunOnceJobsService>();
                services.AddUnitOfWork();
#pragma warning disable 618
                services.AddSingleton<IWebHookProvider>(p => null);
#pragma warning restore 618}
            });


        return hostBuilder;
    }
}