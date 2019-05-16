using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Worker.NServiceBus;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;

namespace SFA.DAS.EAS.Portal.Worker.Startup
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

                    var endpointConfiguration = new EndpointConfiguration(EndpointName.EasPortalWorker)
                        .UseAzureServiceBusTransport(isDevelopment,
                            () => serviceBusConfiguration.ConnectionString, r => { })
                        .UseErrorQueue()
                        .UseInstallers()
                        .UseLicense(serviceBusConfiguration.NServiceBusLicense)
                        .UseMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        .UseServiceCollection(services);

                    return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
                })
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}