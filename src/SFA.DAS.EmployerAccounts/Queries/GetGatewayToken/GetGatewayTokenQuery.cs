namespace SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;

public class GetGatewayTokenQuery : IRequest<GetGatewayTokenQueryResponse>
{
    public string AccessCode { get; set; }
    public string RedirectUrl { get; set; }
}