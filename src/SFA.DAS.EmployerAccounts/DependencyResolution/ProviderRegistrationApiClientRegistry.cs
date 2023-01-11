using System.Net.Http;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class ProviderRegistrationApiClientRegistry : Registry
{
    public ProviderRegistrationApiClientRegistry()
    {
        For<ProviderRegistrationClientApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().ProviderRegistrationsApi);
        For<IProviderRegistrationClientApiConfiguration>().Use(c => c.GetInstance<ProviderRegistrationClientApiConfiguration>());
        For<IProviderRegistrationApiClient>().Use<ProviderRegistrationApiClient>()
            .Ctor<HttpClient>().Is(c => CreateClient(c));
    }

    private HttpClient CreateClient(IContext context)
    {
        HttpClient httpClient = new HttpClientBuilder()
            .WithHandler(new RequestIdMessageRequestHandler())
            .WithHandler(new SessionIdMessageRequestHandler())
            .WithDefaultHeaders()
            .Build();


        return httpClient;
    }
}