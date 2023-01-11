namespace SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;

public class GetGatewayInformationQuery : IAsyncRequest<GetGatewayInformationResponse>
{
    public string ReturnUrl { get; set; }
}