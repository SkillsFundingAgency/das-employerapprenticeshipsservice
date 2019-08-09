using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Application.Services.Commitments;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class CommitmentsApi
    {
        public static IServiceCollection AddCommitmentsApi(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            services.AddSingleton(s => hostBuilderContext.Configuration.GetSection<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApi));
            services.AddTransient<ICommitmentsApiHttpClientFactory, CommitmentsApiHttpClientFactory>();
            services.AddSingleton<ICommitmentsService, CommitmentsService>();
//todo: ApiClientBase has been removed from sfa.das.http, but i suspect SFA.DAS.Commitments.Api.Client references the old sfa.das.http and therefore needs apiclientbase
// could update the commitments client, but v2!
//could throw the client and use resthtppclient direct. or is the v2 client talking to the old api?
            return services;
        }
    }
}
