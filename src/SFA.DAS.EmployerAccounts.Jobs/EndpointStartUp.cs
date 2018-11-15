using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs
{
    public class EndpointStartup 
    {
        private readonly IContainer _container;
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
        private IEndpointInstance _endpoint;

        public EndpointStartup(IContainer container, EmployerAccountsConfiguration employerAccountsConfiguration)
        {
            _container = container;
            _employerAccountsConfiguration = employerAccountsConfiguration;
        }

        public async Task StartAsync()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.Jobs")
                .UseAzureServiceBusTransport(() => _employerAccountsConfiguration.ServiceBusConnectionString)
                .UseLicense(_employerAccountsConfiguration.NServiceBusLicense)
                .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseSendOnly()
                .UseStructureMapBuilder(_container);

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _container.Configure(c => c.For<IMessageSession>().Use(_endpoint));
        }

        public Task StopAsync()
        {
            return _endpoint.Stop();
        }
    }
}