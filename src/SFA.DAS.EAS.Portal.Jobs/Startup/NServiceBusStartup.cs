using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.EAS.Portal.Startup;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.EAS.Portal.Jobs.Startup
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

                    var endpointConfiguration = new EndpointConfiguration(EndpointName.EasPortalJobs)
                        .UseAzureServiceBusTransport(isDevelopment,
                            () => portalConfiguration.ServiceBusConnectionString, r => { })
                        .UseInstallers()
                        .UseLicense(portalConfiguration.NServiceBusLicense)
                        .UseMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        //.UseOutbox()
                        //.UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                        .UseInstallers();
                        //.UseStructureMapBuilder(container)
                        //.UseUnitOfWork();

                    var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                    return endpoint;
                })
                //todo: what's this doing?
                .AddHostedService<NServiceBusHostedService>();

            //return services
            //    .AddSingleton(s =>
            //    {
            //        var configuration = s.GetService<IConfiguration>();
            //        //var container = s.GetService<IContainer>();
            //        var hostingEnvironment = s.GetService<IHostingEnvironment>();
            //        var configurationSection = configuration.GetEmployerFinanceSection<EmployerFinanceConfiguration>();
            //        var isDevelopment = hostingEnvironment.IsDevelopment();

            //        var endpointConfiguration = new EndpointConfiguration(EndpointName.EmployerFinanceV2Jobs)
            //            .UseAzureServiceBusTransport(isDevelopment, () => configurationSection.ServiceBusConnectionString)
            //            .UseInstallers()
            //            .UseLicense(configurationSection.NServiceBusLicense)
            //            .UseMessageConventions()
            //            .UseNewtonsoftJsonSerializer()
            //            .UseNLogFactory()
            //            //not needed just to consume events and write to cosmosdb, but...
            //            //https://github.com/SkillsFundingAgency/das-provider-relationships/blob/master/src/SFA.DAS.ProviderRelationships/ReadStore/Models/AccountProviderLegalEntity.cs#L43
            //            //.UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
            //            // seems to be just for cleanup job, which we won't be using
            //            //.UseStructureMapBuilder(container)
            //            .UseSendOnly();

            //        var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            //        return endpoint;
            //    })
            //    .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
            //    .AddHostedService<NServiceBusHostedService>();
        }
    }
}