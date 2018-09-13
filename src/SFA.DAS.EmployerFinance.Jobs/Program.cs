using System.Data.Common;
using Microsoft.Azure.WebJobs;
using NServiceBus;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Jobs.DependencyResolution;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.NServiceBus.NewtonsoftSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;

namespace SFA.DAS.EmployerFinance.Jobs
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
            
            config.UseTimers();

            var host = new JobHost(config);

            host.Call(typeof(Program).GetMethod(nameof(AsyncMain)));
            host.RunAndBlock();
        }

        [NoAutomaticTrigger]
        public static async Task AsyncMain(CancellationToken cancellationToken)
        {
            var container = IoC.Initialize();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.Jobs")
                .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .SetupLicense(container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
                .SetupMsSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .SetupNewtonsoftSerializer()
                .SetupNLogFactory()
                .SetupSendOnly()
                .SetupStructureMapBuilder(container);

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            container.Configure(c =>
            {
                c.For<IMessageSession>().Use(endpoint);
            });

            ServiceLocator.Initialize(container);
        }
    }
}