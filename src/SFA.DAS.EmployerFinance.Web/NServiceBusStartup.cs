using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Startup;
using SFA.DAS.EmployerFinance.Web.App_Start;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.EmployerFinance.Web
{
    public class NServiceBusStartup : IStartup
    {
        private IEndpointInstance _endpoint;

        public async Task StartAsync()
        {
            var container = StructuremapMvc.StructureMapDependencyScope.Container;

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.Web")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue("SFA.DAS.EmployerAccounts.Web-errors")
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense))
                .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox(true)
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