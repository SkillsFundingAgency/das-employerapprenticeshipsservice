using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ProviderServices
    {
        public static IServiceCollection AddProviderServices(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            var apiClientConfig = hostBuilderContext.Configuration.GetSection<ApprenticeshipInfoServiceApiConfiguration>(ConfigurationKeys.ApprenticeshipInfoServiceApi);

            services.AddTransient<IProviderApiClient>(sp => new ProviderApiClient(apiClientConfig.BaseUrl));
            // if we want to support config changes on the fly, we could create wrapper class that accepts IOptionsMonitor and picks out url

            return services;
        }
    }
}