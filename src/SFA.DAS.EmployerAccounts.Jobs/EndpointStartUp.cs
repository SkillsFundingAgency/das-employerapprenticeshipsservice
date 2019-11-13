﻿using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs
{
    public class EndpointStartup 
    {
        private readonly IContainer _container;
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
        private readonly IEnvironmentService _environmentService;
        private IEndpointInstance _endpoint;

        public EndpointStartup(IContainer container, EmployerAccountsConfiguration employerAccountsConfiguration, IEnvironmentService environmentService)
        {
            _container = container;
            _employerAccountsConfiguration = employerAccountsConfiguration;
            _environmentService = environmentService;
        }

        public async Task StartAsync()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.Jobs")
                .UseErrorQueue("SFA.DAS.EmployerAccounts.Jobs-errors")
                .UseAzureServiceBusTransport(() => _employerAccountsConfiguration.ServiceBusConnectionString, _container)
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