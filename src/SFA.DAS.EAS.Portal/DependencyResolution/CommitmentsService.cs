using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using System;
using System.Net.Http;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class CommitmentsService
    {
        public static IServiceCollection AddCommitmentsApiConfiguration(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            services.AddSingleton(s => hostBuilderContext.Configuration.GetSection<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApi));
            services.AddTransient<ICommitmentsApiClientConfiguration, CommitmentsApiClientConfiguration>();
            services.AddTransient<IProviderCommitmentsApi>(s => new ProviderCommitmentsApi(GetHttpClient(s), s.GetService<CommitmentsApiClientConfiguration>()));

            return services;
        }

        public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration, IJwtClientConfiguration
        {
            public string BaseUrl { get; set; }
            public string ClientToken { get; set; }
        }
        private static HttpClient GetHttpClient(IServiceProvider provider)
        {
            var config = provider.GetService<CommitmentsApiClientConfiguration>();

            var httpClient = new HttpClientBuilder()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
                .WithDefaultHeaders()
                .Build();

            return httpClient;
        }
    }
}
