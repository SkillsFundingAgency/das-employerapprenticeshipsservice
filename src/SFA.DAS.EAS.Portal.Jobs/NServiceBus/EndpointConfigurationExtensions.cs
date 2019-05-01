using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace SFA.DAS.EAS.Portal.Jobs.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        //todo: move into sfa.das.nservicebus (with cleanupjob registry)
        //todo: name? UseCoreDependencyInjection?
        public static EndpointConfiguration UseServiceCollection(this EndpointConfiguration config, IServiceCollection services)
        {
            config.UseContainer<ServicesBuilder>(c => c.ExistingServices(services));
            return config;
        }
    }
}
