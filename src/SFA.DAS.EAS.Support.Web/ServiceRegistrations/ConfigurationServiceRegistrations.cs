using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        var supportEasStr = configuration.GetValue<string>("SFA.DAS.Support.EAS");
        var supportConfig = JsonSerializer.Deserialize<WebConfiguration>(supportEasStr);

        var employerAccApiClientConfigStr = configuration.GetValue<string>("SFA.DAS.EmployerAccounts.Api.Client");
        var employerAccApiClientConfig = JsonSerializer.Deserialize<EmployerAccountsApiClientConfiguration>(employerAccApiClientConfigStr);

        var tokenServiceApiClientConfigStr = configuration.GetValue<string>("SFA.DAS.TokenServiceApiClient");
        var tokenServiceApiClientConfig = JsonSerializer.Deserialize<TokenServiceApiClientConfiguration>(tokenServiceApiClientConfigStr);

        var accApiConfigStr = configuration.GetValue<string>("SFA.DAS.EmployerAccountAPI");
        var accApiConfig = JsonSerializer.Deserialize<AccountApiConfiguration>(accApiConfigStr);

        services.AddSingleton<IPayRefHashingService, PayRefHashingService>(c => new PayRefHashingService(supportConfig.HashingService.AllowedCharacters, supportConfig.HashingService.Hashstring));
        services.AddSingleton<IEmployerAccountsApiClientConfiguration, EmployerAccountsApiClientConfiguration>(c => employerAccApiClientConfig);
        services.AddSingleton<ITokenServiceApiClientConfiguration, TokenServiceApiClientConfiguration>(c => tokenServiceApiClientConfig);
        services.AddSingleton<IHmrcApiClientConfiguration, HmrcApiClientConfiguration>(c => supportConfig.LevySubmission.HmrcApi);
        services.AddSingleton<IAccountApiConfiguration, AccountApiConfiguration>(c => accApiConfig);

        return services;
    }
}
