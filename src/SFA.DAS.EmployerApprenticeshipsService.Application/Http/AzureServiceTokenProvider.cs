using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Http;

public interface IAzureServiceTokenProvider
{
    Task<string> GetTokenAsync();
}

public class AzureServiceTokenProvider<TConfiguration>(TConfiguration configuration) : IAzureServiceTokenProvider
    where TConfiguration : IManagedIdentityClientConfiguration
{
    private readonly ChainedTokenCredential _azureServiceTokenProvider = new(
        new ManagedIdentityCredential(),
        new AzureCliCredential(),
        new VisualStudioCodeCredential(),
        new VisualStudioCredential()
    );

    public async Task<string> GetTokenAsync()
    {
        return (await _azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: [configuration.IdentifierUri]))).Token;
    }
}