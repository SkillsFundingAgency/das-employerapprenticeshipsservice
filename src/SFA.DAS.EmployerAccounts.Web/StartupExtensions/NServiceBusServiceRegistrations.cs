using Microsoft.Extensions.DependencyInjection.Extensions;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Services;
using SFA.DAS.UnitOfWork.Pipeline;
using Endpoint = NServiceBus.Endpoint;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class NServiceBusServiceRegistrations
{
    private const string EndPointName = "SFA.DAS.EmployerAccounts";

    public static UpdateableServiceProvider StartNServiceBus(this IServiceCollection services, IConfiguration configuration, EmployerAccountsConfiguration employerAccountsConfiguration)
    {
        var endpointConfiguration = new EndpointConfiguration(EndPointName)
            .UseInstallers()
            .UseErrorQueue($"{EndPointName}-errors")
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer()
            .UseOutbox(true)
            .UseSqlServerPersistence(() => DatabaseExtensions.GetSqlConnection(configuration.IsDev(), employerAccountsConfiguration.DatabaseConnectionString))
            .UseUnitOfWork();

        // https://github.com/twenzel/NServiceBus.MSDependencyInjection/blob/master/README.md
       
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

        if (configuration.IsDev())
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

        return container;
    }

    public static IServiceCollection AddNServiceBusClientUnitOfWork(this IServiceCollection services)
    {
        services.TryAddScoped<IEventPublisher, EventPublisher>();

        return services.AddUnitOfWork()
            .AddScoped<IUnitOfWork, UnitOfWork.NServiceBus.Features.ClientOutbox.Pipeline.UnitOfWork>()
            .AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
    }
}

