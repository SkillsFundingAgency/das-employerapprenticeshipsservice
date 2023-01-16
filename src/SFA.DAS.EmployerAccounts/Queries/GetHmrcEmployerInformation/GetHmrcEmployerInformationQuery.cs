namespace SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;

public class GetHmrcEmployerInformationQuery :IRequest<GetHmrcEmployerInformationResponse>
{
    public string AuthToken { get; set; }
}