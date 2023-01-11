namespace SFA.DAS.EmployerAccounts.Queries.GetCharity;

public class GetCharityQueryRequest : IAsyncRequest<GetCharityQueryResponse>
{
    public int RegistrationNumber { get; set; }
}