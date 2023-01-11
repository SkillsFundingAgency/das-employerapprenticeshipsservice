namespace SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;

public class GetHmrcEmployerInformationQuery :IAsyncRequest<GetHmrcEmployerInformationResponse>
{
    public string AuthToken { get; set; }
}