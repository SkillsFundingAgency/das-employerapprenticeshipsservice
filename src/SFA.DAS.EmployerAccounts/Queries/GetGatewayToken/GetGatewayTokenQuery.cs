namespace SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;

public class GetGatewayTokenQuery : IAsyncRequest<GetGatewayTokenQueryResponse>
{
    public string AccessCode { get; set; }
    public string RedirectUrl { get; set; }
}