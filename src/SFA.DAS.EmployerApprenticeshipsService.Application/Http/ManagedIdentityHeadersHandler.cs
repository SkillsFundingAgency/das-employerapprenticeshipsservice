using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace SFA.DAS.EAS.Application.Http;

public sealed class ManagedIdentityHeadersHandler : DelegatingHandler
{
    private readonly IManagedIdentityTokenGenerator _tokenGenerator;

    public ManagedIdentityHeadersHandler(IManagedIdentityTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _tokenGenerator.Generate().ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
