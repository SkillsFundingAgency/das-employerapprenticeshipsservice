using SFA.DAS.Hmrc;

namespace SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;

public class GetGatewayInformationHandler : IAsyncRequestHandler<GetGatewayInformationQuery, GetGatewayInformationResponse>
{
    private readonly IHmrcService _hmrcService;

    public GetGatewayInformationHandler(IHmrcService hmrcService)
    {
        _hmrcService = hmrcService;
    }

    public Task<GetGatewayInformationResponse> Handle(GetGatewayInformationQuery message)
    {
        var returnUrl = _hmrcService.GenerateAuthRedirectUrl(message.ReturnUrl);

        return Task.FromResult(new GetGatewayInformationResponse { Url = returnUrl });
    }
}