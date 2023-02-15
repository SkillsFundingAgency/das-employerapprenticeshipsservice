using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness;

public class NServiceBusStartup //: IStartup
{
    private readonly IContainer _container;
    private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
    private IEndpointInstance _endpoint;

    public NServiceBusStartup(IContainer container, EmployerAccountsConfiguration employerAccountsConfiguration)
    {
        _container = container;
        _employerAccountsConfiguration = employerAccountsConfiguration;
    }

    public async Task StartAsync()
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.MessageHandlers")
            .UseAzureServiceBusTransport(() => _employerAccountsConfiguration.ServiceBusConnectionString, _container)
            .UseErrorQueue("SFA.DAS.EmployerAccounts.MessageHandlers-errors")
            .UseInstallers()
            .UseLicense(WebUtility.HtmlDecode(_employerAccountsConfiguration.NServiceBusLicense))
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