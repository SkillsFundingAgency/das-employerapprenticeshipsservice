using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Application.Services.Commitments;
using SFA.DAS.EAS.Portal.Application.Services.Commitments.Http;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class CommitmentsApi
    {
        public static IServiceCollection AddCommitmentsApi(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            services.AddSingleton(s => hostBuilderContext.Configuration.GetSection<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApi));
            services.AddTransient<ICommitmentsApiHttpClientFactory, CommitmentsApiHttpClientFactory>();
            services.AddSingleton<ICommitmentsService, CommitmentsService>();
            return services;
        }
    }
}
