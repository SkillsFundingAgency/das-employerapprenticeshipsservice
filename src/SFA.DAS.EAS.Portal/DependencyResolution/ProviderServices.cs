using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ProviderServices
    {
        public static IServiceCollection AddProviderServices(this IServiceCollection services) //, IConfiguration configuration)
        {
            //todo: don't want to repeatedly do this. better way?
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            var apiClientConfig = configuration.GetSection<CommitmentsService.CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApi);

            services.AddTransient<IProviderApiClient>(sp => new ProviderApiClient(apiClientConfig.BaseUrl));
            // if we want to support config changes on the fly, we could create wrapper class that accepts IOptionsMonitor and picks out url

            return services;
        }
    }
}