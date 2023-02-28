using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Extensions;
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

    public static IServiceCollection AddEmployerFinanceDbContext(this IServiceCollection services)
    {
        services.AddTransient(provider =>
        {
            var config = provider.GetService<IConfiguration>();
            var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>();
            optionsBuilder.UseSqlServer(config.GetValue<string>($"{ConfigurationKeys.EmployerAccounts}:DatabaseConnectionString"));
            return new EmployerAccountsDbContext(optionsBuilder.Options);
        });

        services.AddTransient(provider => new Lazy<EmployerAccountsDbContext>(provider.GetService<EmployerAccountsDbContext>()));

        return services;
    }

    public static IServiceCollection AddNServiceBus(this IServiceCollection services)
    {
        return services
            .AddSingleton(p =>
            {
                var hostingEnvironment = p.GetService<IHostEnvironment>();
                var configuration = p.GetService<IConfiguration>();
                var accountsConfiguration = configuration
                    .GetSection(ConfigurationKeys.EmployerAccounts)
                    .Get<EmployerAccountsConfiguration>();

                var isDevelopment = hostingEnvironment.IsDevelopment();

                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .UseErrorQueue($"{EndpointName}-errors")
                    .UseInstallers()
                    .UseLicense(accountsConfiguration.NServiceBusLicense)
                    .UseMessageConventions()
                    .UseNewtonsoftJsonSerializer()
                    .UseNLogFactory()
                    .UseOutbox(true)
                    .UseSqlServerPersistence(() => DatabaseExtensions.GetSqlConnection(accountsConfiguration.DatabaseConnectionString))
                    .UseSendOnly()
                    .UseServicesBuilder(new UpdateableServiceProvider(services));

                if (isDevelopment)
                {
                    endpointConfiguration.UseLearningTransport();
                }
                else
                {
                    endpointConfiguration.UseAzureServiceBusTransport(accountsConfiguration.ServiceBusConnectionString);
                }

                return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            })
            .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}