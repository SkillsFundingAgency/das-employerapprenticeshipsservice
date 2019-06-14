using System.Data.Common;
using Microsoft.Azure.WebJobs;
using NServiceBus;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers
{
    public class Program
    {
        public static void Main()
        {
            var container = IoC.Initialize();

            var isDevelopment = container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL);

            var config = new JobHostConfiguration();

            if (isDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);

            host.Call(typeof(Program).GetMethod(nameof(AsyncMain)), new { isDevelopment });
            host.RunAndBlock();
        }

        [NoAutomaticTrigger]
        public static async Task AsyncMain(CancellationToken cancellationToken, bool isDevelopment, IContainer container)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.MessageHandlers")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerAccountsConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(container.GetInstance<EmployerAccountsConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(container)
                .UseUnitOfWork();

            
            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(3000, cancellationToken).ConfigureAwait(false);
            }

            await endpoint.Stop().ConfigureAwait(false);
            container.Dispose();
        }
    }
}
