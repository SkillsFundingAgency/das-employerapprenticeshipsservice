using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Jobs.DependencyResolution;
using SFA.DAS.EmployerFinance.Startup;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Jobs
{
    public class NServiceBusStartup : IStartup
    {
        private readonly IContainer _container;
        private IEndpointInstance _endpoint;

        public NServiceBusStartup(IContainer container)
        {
            _container = container;
        }

        public async Task StartAsync()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.Jobs")
                .UseAzureServiceBusTransport(() => _container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .UseLicense(_container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseSendOnly()
                .UseStructureMapBuilder(_container);

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });

            ServiceLocator.Initialize(_container);
        }

        public async Task StopAsync()
        {
            await _endpoint.Stop();
        }
    }

    //public class JobStartup : IStartup
    //{
    //    private readonly IContainer _container;
    //    private static IEndpointInstance _endpoint;

    //    public JobStartup()
    //    {
    //        //_container = Program.GetContainer();
    //    }

    //    public Task StartAsync()
    //    {
    //        var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(DAS.Configuration.Environment.Local);
    //        var config = new JobHostConfiguration();

    //        if (isDevelopment)
    //        {
    //            config.UseDevelopmentSettings();
    //        }

    //        config.UseTimers();

    //        var host = new JobHost(config);

    //        host.Call(typeof(Program).GetMethod(nameof(AsyncMain)));
    //        host.RunAndBlock();
    //        return Task.CompletedTask;
    //    }

    //    public Task StopAsync()
    //    {
    //        return _endpoint.Stop();
    //    }

    //    [NoAutomaticTrigger]
    //    public async Task AsyncMain(CancellationToken cancellationToken)
    //    {
    //        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.Jobs")
    //            .UseAzureServiceBusTransport(() => _container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
    //            .UseLicense(_container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
    //            .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
    //            .UseNewtonsoftJsonSerializer()
    //            .UseNLogFactory()
    //            .UseSendOnly()
    //            .UseStructureMapBuilder(_container);

    //        _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

    //        _container.Configure(c =>
    //        {
    //            c.For<IMessageSession>().Use(_endpoint);
    //        });

    //        ServiceLocator.Initialize(_container);
    //    }
    //}
}
