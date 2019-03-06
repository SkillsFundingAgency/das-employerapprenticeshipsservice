using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Startup;
using SFA.DAS.EmployerFinance.Web.App_Start;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.EmployerFinance.Web
{
    public class NServiceBusStartup : IStartup
    {
        private IEndpointInstance _endpoint;

        public async Task StartAsync()
        {
            var container = StructuremapMvc.StructureMapDependencyScope.Container;

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.Web")
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

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });
        }

        public Task StopAsync()
        {
            return _endpoint.Stop();
        }
    }
}