using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.EAS.Portal.Worker.NServiceBus;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Startup
{
    public static class NServiceBusStartup
    {
        public static IServiceCollection AddDasNServiceBus(this IServiceCollection services)
        {
            return services
                .AddSingleton(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var hostingEnvironment = s.GetService<IHostingEnvironment>();
                    var serviceBusConfiguration = configuration.GetPortalSection<ServiceBusConfiguration>(PortalSections.ServiceBus);
                    var isDevelopment = hostingEnvironment.IsDevelopment();

                    var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Portal.Worker.TestHarness")
                        .UseAzureServiceBusTransport(isDevelopment, serviceBusConfiguration.ConnectionString)
                        .UseErrorQueue("SFA.DAS.EAS.Portal.Worker.TestHarness-errors")
                        .UseInstallers()
                        .UseLicense(serviceBusConfiguration.NServiceBusLicense)
                        .UsePortalMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        .UseSendOnly()
                    ;

                    return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
                })
                .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }

        public static EndpointConfiguration UseAzureServiceBusTransport(
           this EndpointConfiguration endpointConfiguration,
           bool isDevelopment,
           string connectionString
       )
        {
            if (isDevelopment)
            {
                var transport = endpointConfiguration.UseTransport<LearningTransport>();
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
            }

            else
            {
                endpointConfiguration
                    .UseAzureServiceBusTransport(connectionString, r => { })
                ;
            }

            return endpointConfiguration;
        }
    }
}