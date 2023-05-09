using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Http;

public class ManagedIdentityTokenGenerator : IManagedIdentityTokenGenerator
{
    private readonly IManagedIdentityClientConfiguration _config;
    public ManagedIdentityTokenGenerator(IManagedIdentityClientConfiguration config)
    {
        _config = config;
    }

    public Task<string> Generate()
    {
        var azureServiceTokenProvider = new AzureServiceTokenProvider();
        return azureServiceTokenProvider.GetAccessTokenAsync(_config.IdentifierUri);
    }
}


