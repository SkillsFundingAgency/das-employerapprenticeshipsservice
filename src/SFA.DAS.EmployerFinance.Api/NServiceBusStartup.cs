using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Startup;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerFinance.Api
{
    public class NServiceBusStartup : IStartup
    {
        private IEndpointInstance _endpoint;

        public async Task StartAsync()
        {
            var container = GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.Api")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue("SFA.DAS.EmployerFinance.Api-errors")
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense))
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