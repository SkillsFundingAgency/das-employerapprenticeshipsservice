namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;

public class GetUserAccountsQuery : IAsyncRequest<GetUserAccountsQueryResponse>
{
    public string UserRef { get; set; }
}