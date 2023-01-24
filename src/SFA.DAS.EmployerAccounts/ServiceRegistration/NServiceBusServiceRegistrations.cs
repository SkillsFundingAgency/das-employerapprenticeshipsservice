using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using Endpoint = NServiceBus.Endpoint;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class NServiceBusServiceRegistrations
{
    private const string EndPointName = "SFA.DAS.EmployerAccounts";

    public static void StartNServiceBus(this IServiceCollection services, EmployerAccountsConfiguration employerAccountsConfiguration, bool isDevOrLocal)
    {
        var endpointConfiguration = new EndpointConfiguration(EndPointName)
            .UseErrorQueue($"{EndPointName}-errors")
            .UseInstallers()
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer()
            .UseNLogFactory()
            .UseOutbox(true)
            .UseSqlServerPersistence(() => DatabaseExtensions.GetSqlConnection(isDevOrLocal, employerAccountsConfiguration.DatabaseConnectionString))
            .UseUnitOfWork();

        //// https://github.com/twenzel/NServiceBus.MSDependencyInjection/blob/master/README.md

        UpdateableServiceProvider container = null;

        endpointConfiguration.UseContainer<ServicesBuilder>(customisations =>
        {
            customisations.ExistingServices(services);
            customisations.ServiceProviderFactory(sc =>
            {
                container = new UpdateableServiceProvider(sc);
                return container;
            });
        });

        if (isDevOrLocal)
        {
            endpointConfiguration.UseLearningTransport();
        }
        else
        {
            endpointConfiguration.UseAzureServiceBusTransport(employerAccountsConfiguration.MessageServiceBusConnectionString, r => { });
        }

        if (!string.IsNullOrEmpty(employerAccountsConfiguration.NServiceBusLicense))
        {
            endpointConfiguration.License(employerAccountsConfiguration.NServiceBusLicense);
        }

        var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

        services.AddSingleton(p => endpoint)
            .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}