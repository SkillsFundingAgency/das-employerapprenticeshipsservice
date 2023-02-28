using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;

public static class ServiceCollectionExtensions
{
    private const string EndpointName = "SFA.DAS.EmployerAccounts.MessageHandlers";

    public static IServiceCollection AddNServiceBus(this IServiceCollection services)
    {
        return services
            .AddSingleton(p =>
            {
                var container = p.GetService<IContainer>();
                var hostingEnvironment = p.GetService<IHostEnvironment>();
                var configuration = p.GetService<EmployerAccountsConfiguration>();
                var isDevelopment = hostingEnvironment.IsDevelopment();

                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .UseErrorQueue($"{EndpointName}-errors")
                    .UseInstallers()
                    .UseLicense(configuration.NServiceBusLicense)
                    .UseMessageConventions()
                    .UseNewtonsoftJsonSerializer()
                    .UseNLogFactory()
                    .UseOutbox()
                    .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                    .UseServicesBuilder(new UpdateableServiceProvider(services))
                    .UseUnitOfWork();

                if (isDevelopment)
                {
                    endpointConfiguration.UseLearningTransport(s => s.AddRouting());
                }
                else
                {
                    endpointConfiguration.UseAzureServiceBusTransport(configuration.ServiceBusConnectionString, s => s.AddRouting());
                }

                return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            })
            .AddHostedService<NServiceBusHostedService>();
    }
}