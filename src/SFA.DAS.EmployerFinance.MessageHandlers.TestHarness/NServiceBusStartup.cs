using System.Net;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using StructureMap;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.EmployerFinance.MessageHandlers.TestHarness
{
    public class NServiceBusStartup //: IStartup
    {
        private readonly IContainer _container;
        private readonly EmployerFinanceConfiguration _employerFinanceConfiguration;
        private IEndpointInstance _endpoint;

        public NServiceBusStartup(IContainer container, EmployerFinanceConfiguration employerFinanceConfiguration)
        {
            _container = container;
            _employerFinanceConfiguration = employerFinanceConfiguration;
        }

        public async Task StartAsync()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.MessageHandlers")
                .UseAzureServiceBusTransport(() => _employerFinanceConfiguration.ServiceBusConnectionString, _container)
                .UseErrorQueue("SFA.DAS.EmployerFinance.MessageHandlers-errors")
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(_employerFinanceConfiguration.NServiceBusLicense))
                .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(_container)
                .UseUnitOfWork();

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _container.Configure(c => c.For<IMessageSession>().Use(_endpoint));
        }

        public Task StopAsync()
        {
            return _endpoint.Stop();
        }
    }
}
