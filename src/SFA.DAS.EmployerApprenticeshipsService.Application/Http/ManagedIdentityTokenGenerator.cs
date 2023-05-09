using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Http;

public class ManagedIdentityTokenGenerator : IManagedIdentityTokenGenerator
{
    private readonly IManagedIdentityClientConfiguration _config;

    public ManagedIdentityTokenGenerator(IManagedIdentityClientConfiguration config)
    {
        _config = config;
    }

    public async Task<string> Generate()
    {
        var tokenCredential = new DefaultAzureCredential();
        
        var accessToken = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new string[] { _config.IdentifierUri + "/.default" }) { }
        );

        return accessToken.Token;
    }
}


