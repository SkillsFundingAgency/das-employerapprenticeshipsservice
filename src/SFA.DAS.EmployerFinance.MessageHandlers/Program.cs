using System.Data.Common;
using Microsoft.Azure.WebJobs;
using NServiceBus;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.MessageHandlers.DependencyResolution;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.EmployerFinance.MessageHandlers
{
    public class Program
    {
        public static void Main()
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);
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
        public static async Task AsyncMain(CancellationToken cancellationToken, bool isDevelopment)
        {
            var container = IoC.Initialize();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.MessageHandlers")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
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
