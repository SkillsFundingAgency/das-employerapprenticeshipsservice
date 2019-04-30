using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;

//using SFA.DAS.EmployerFinance.Configuration;
//using SFA.DAS.EmployerFinance.Extensions;
//using SFA.DAS.EmployerFinance.Startup;
//using SFA.DAS.NServiceBus;
//using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
//using SFA.DAS.NServiceBus.NLog;
//using SFA.DAS.NServiceBus.StructureMap;
//using StructureMap;

namespace SFA.DAS.EAS.Portal.Jobs.TestHarness.Startup
{
    public static class NServiceBusStartup
    {
        public static IServiceCollection AddDasNServiceBus(this IServiceCollection services)
        {
            return services
                .AddSingleton(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    //var container = s.GetService<IContainer>();
                    var hostingEnvironment = s.GetService<IHostingEnvironment>();
                    var portalConfiguration = configuration.GetPortalSection<PortalConfiguration>();
                    var isDevelopment = hostingEnvironment.IsDevelopment();

                    var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerApprenticeshipService.Portal.Jobs.TestHarness")
                        .UseAzureServiceBusTransport(isDevelopment,  () => portalConfiguration.ServiceBusConnectionString, r => { })
                        .UseInstallers()
                        .UseLicense(portalConfiguration.NServiceBusLicense)
                        .UseMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        //.UseStructureMapBuilder(container)
                        .UseSendOnly();

                    var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                    return endpoint;
                })
                .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}