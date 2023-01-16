namespace SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;

public class GetGatewayInformationQuery : IRequest<GetGatewayInformationResponse>
{
    public string ReturnUrl { get; set; }
}