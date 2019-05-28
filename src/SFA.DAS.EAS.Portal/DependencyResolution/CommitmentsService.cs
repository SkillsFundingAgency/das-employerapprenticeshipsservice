using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using System;
using System.Net.Http;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class CommitmentsService
    {
        public static IServiceCollection AddCommitmentsApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //todo: we can probably get away with not leaving the config in the container
            services.AddSingleton(s => configuration.GetSection<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApi));
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
