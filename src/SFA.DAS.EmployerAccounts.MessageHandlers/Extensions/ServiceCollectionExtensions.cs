using System.Net;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
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
            .AddSingleton(provider =>
            {
                var hostingEnvironment = provider.GetService<IHostEnvironment>();
                var configuration = provider.GetService<EmployerAccountsConfiguration>();
                var isDevelopment = hostingEnvironment.IsDevelopment();

                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .ConfigureServiceBusTransport(() => configuration.ServiceBusConnectionString, isDevelopment)
                    .UseErrorQueue($"{EndpointName}-errors")
                    .UseInstallers()
                    .UseSqlServerPersistence(() => DatabaseExtensions.GetSqlConnection(configuration.DatabaseConnectionString))
                    .UseNewtonsoftJsonSerializer()
                    .UseOutbox()
                    .UseUnitOfWork()
                    .UseServicesBuilder(new UpdateableServiceProvider(services));

                if (!string.IsNullOrEmpty(configuration.NServiceBusLicense))
                {
                    var decodedLicence = WebUtility.HtmlDecode(configuration.NServiceBusLicense);
                    endpointConfiguration.UseLicense(decodedLicence);
                }

                var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                return endpoint;
            })
            .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}