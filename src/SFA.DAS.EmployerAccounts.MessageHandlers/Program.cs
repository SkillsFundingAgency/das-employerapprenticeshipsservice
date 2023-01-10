using System.Data.Common;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers
{
    public class Program
    {
        public static async Task Main()
        {
            var container = IoC.Initialize();

            var isDevelopment = container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL);

            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureWebJobs();

            if (isDevelopment)
            {
                hostBuilder.UseEnvironment("development");
            }

            using var host = hostBuilder.Build();

            var jobHost = host.Services.GetService(typeof(IJobHost)) as JobHost;

            await host.StartAsync();
            await jobHost.CallAsync(typeof(Program).GetMethod(nameof(AsyncMain)), new
            {
                container
            });
            await host.RunAsync();
        }

        [NoAutomaticTrigger]
        public static async Task AsyncMain(CancellationToken cancellationToken, IContainer container)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.MessageHandlers")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerAccountsConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue("SFA.DAS.EmployerAccounts.MessageHandlers-errors")
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(container.GetInstance<EmployerAccountsConfiguration>().NServiceBusLicense))
                .UseSqlServerPersistence(container.GetInstance<DbConnection>)
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(container)
                .UseUnitOfWork();

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            container.Configure(x =>
            {
                x.For<IMessageSession>().Use(endpoint);
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(3000, cancellationToken).ConfigureAwait(false);
            }

            await endpoint.Stop().ConfigureAwait(false);
            container.Dispose();
        }
    }
}
