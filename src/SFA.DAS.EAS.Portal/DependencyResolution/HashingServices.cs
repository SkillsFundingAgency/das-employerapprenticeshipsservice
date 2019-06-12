using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class HashingServices
    {
        public static IServiceCollection AddHashingServices(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            var hashServiceConfig = hostBuilderContext.Configuration.GetPortalSection<HashingServiceConfiguration>(PortalSections.HashingService);
            services.AddSingleton<IHashingService>(s =>
                new HashingService.HashingService(hashServiceConfig.AccountLegalEntityPublicAllowedCharacters,
                    hashServiceConfig.AccountLegalEntityPublicHashstring));
            
            return services;
        }
    }
}