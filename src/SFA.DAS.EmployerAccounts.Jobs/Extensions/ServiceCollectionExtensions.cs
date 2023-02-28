using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;

namespace SFA.DAS.EmployerAccounts.Jobs.Extensions;

public static class ServiceCollectionExtensions
{
    private const string EndpointName = "SFA.DAS.EmployerAccounts.Jobs";

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
                    .UseOutbox(true)
                    .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                    .UseSendOnly()
                    .UseServicesBuilder(new UpdateableServiceProvider(services));

                if (isDevelopment)
                {
                    endpointConfiguration.UseLearningTransport();
                }
                else
                {
                    endpointConfiguration.UseAzureServiceBusTransport(configuration.ServiceBusConnectionString);
                }

                return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            })
            .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}