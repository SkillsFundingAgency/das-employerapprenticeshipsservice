using System.Diagnostics.CodeAnalysis;
using System.Net;
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

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public enum ServiceBusEndpointType
{
    Api,
    Web
}

[ExcludeFromCodeCoverage]
public static class NServiceBusServiceRegistrations
{
    public static void StartNServiceBus(this UpdateableServiceProvider services, bool isDevOrLocal, ServiceBusEndpointType endpointType)
    {
        var endPointName = $"SFA.DAS.EmployerAccounts.{endpointType}";
        var employerAccountsConfiguration = services.GetService<EmployerAccountsConfiguration>();
       
        var databaseConnectionString = employerAccountsConfiguration.DatabaseConnectionString;

        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            throw new InvalidConfigurationValueException("DatabaseConnectionString");
        }

        var allowOutboxCleanup = endpointType = ServiceBusEndpointType.Api;

        var endpointConfiguration = new EndpointConfiguration(endPointName)
            .UseErrorQueue($"{endPointName}-errors")
            .UseInstallers()
            .UseMessageConventions()
            .UseServicesBuilder(services)
            .UseNewtonsoftJsonSerializer()
            .UseOutbox()
            .UseSqlServerPersistence(() => DatabaseExtensions.GetSqlConnection(databaseConnectionString))
            .ConfigureServiceBusTransport(() => employerAccountsConfiguration.ServiceBusConnectionString, isDevOrLocal)
            .UseUnitOfWork();

        if (!string.IsNullOrEmpty(employerAccountsConfiguration.NServiceBusLicense))
        {
            var decodedLicence = WebUtility.HtmlDecode(employerAccountsConfiguration.NServiceBusLicense);
            endpointConfiguration.License(decodedLicence);
        }

        var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

        services.AddSingleton(p => endpoint)
            .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}