using System;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Extensions;

public static class ServiceCollectionExtensions
{
    private const string EndpointName = "SFA.DAS.EmployerAccounts.MessageHandlers";

    public static IServiceCollection AddNServiceBus(this IServiceCollection services)
    {
        return services
            .AddSingleton(p =>
            {
                var employerAccountsConfiguration = p.GetService<EmployerAccountsConfiguration>();
                var configuration = p.GetService<IConfiguration>();
                var isLocal = configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);

                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .ConfigureServiceBusTransport(() => employerAccountsConfiguration.ServiceBusConnectionString,
                        isLocal)
                    .UseErrorQueue($"{EndpointName}-errors")
                    .UseInstallers()
                    .UseLicense(WebUtility.HtmlDecode(employerAccountsConfiguration.NServiceBusLicense))
                    .UseSqlServerPersistence(() =>
                        DatabaseExtensions.GetSqlConnection(employerAccountsConfiguration.DatabaseConnectionString))
                    .UseNewtonsoftJsonSerializer()
                    .UseOutbox()
                    .UseUnitOfWork()
                    .UseMessageConventions()
                    .UseServicesBuilder(new UpdateableServiceProvider(services));

                return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            })
            .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}