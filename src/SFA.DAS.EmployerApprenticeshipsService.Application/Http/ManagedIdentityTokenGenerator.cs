using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.Application.Http;

public interface IManagedIdentityTokenGenerator : IGenerateBearerToken { }

public class ManagedIdentityTokenGenerator<T> : IManagedIdentityTokenGenerator where T : IManagedIdentityClientConfiguration
{
    private readonly T _config;

    public ManagedIdentityTokenGenerator(T config) => _config = config;

    public async Task<string> Generate()
    {
        var tokenCredential = new DefaultAzureCredential();
        
        var accessToken = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new[] { _config.IdentifierUri + "/.default" })
        );

        return accessToken.Token;
    }
}