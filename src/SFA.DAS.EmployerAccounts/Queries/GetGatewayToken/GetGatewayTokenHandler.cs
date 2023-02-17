using System.Threading;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;

namespace SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;

public class GetGatewayTokenHandler : IRequestHandler<GetGatewayTokenQuery, GetGatewayTokenQueryResponse>
{
    private readonly IHmrcService _hmrcService;

    public GetGatewayTokenHandler(IHmrcService hmrcService)
    {
        _hmrcService = hmrcService;
    }

    public async Task<GetGatewayTokenQueryResponse> Handle(GetGatewayTokenQuery message, CancellationToken cancellationToken)
    {
        var response = await _hmrcService.GetAuthenticationToken(message.RedirectUrl, message.AccessCode);

        return new GetGatewayTokenQueryResponse
        {
            HmrcTokenResponse = response
        };
    }
}