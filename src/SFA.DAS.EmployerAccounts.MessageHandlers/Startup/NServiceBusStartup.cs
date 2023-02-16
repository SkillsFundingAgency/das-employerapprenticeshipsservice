//using SFA.DAS.EmployerAccounts.Configuration;
//using SFA.DAS.EmployerAccounts.Extensions;
//using SFA.DAS.NServiceBus.Configuration;
//using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
//using SFA.DAS.NServiceBus.Configuration.NLog;
//using SFA.DAS.NServiceBus.Configuration.StructureMap;
//using SFA.DAS.NServiceBus.SqlServer.Configuration;
//using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

//namespace SFA.DAS.EmployerAccounts.MessageHandlers.Startup;

//public class NServiceBusStartup
//{
//    private readonly IContainer _container;
//    private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
//    private IEndpointInstance _endpoint;

//    private const string EndpointName = "SFA.DAS.EmployerAccounts.MessageHandlers";

//    public NServiceBusStartup(IContainer container, EmployerAccountsConfiguration employerAccountsConfiguration)
//    {
//        _container = container;
//        _employerAccountsConfiguration = employerAccountsConfiguration;
//    }

//    public async Task StartAsync()
//    {
//        var endpointConfiguration = new EndpointConfiguration(EndpointName)
//            .UseErrorQueue($"{EndpointName}-errors")
//            .UseInstallers()
//            .UseLicense(WebUtility.HtmlDecode(_employerAccountsConfiguration.NServiceBusLicense))
//            .UseSqlServerPersistence(_container.GetInstance<DbConnection>)
//            .UseNewtonsoftJsonSerializer()
//            .UseNLogFactory()
//            .UseOutbox()
//            .UseStructureMapBuilder(_container)
//            .UseUnitOfWork()
//            .UseAzureServiceBusTransport(() => _employerAccountsConfiguration.ServiceBusConnectionString, _container);

//        _endpoint = await Endpoint.Start(endpointConfiguration);

//        _container.Configure(c => c.For<IMessageSession>().Use(_endpoint));
//    }

//    public Task StopAsync()
//    {
//        return _endpoint.Stop();
//    }
//}