namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;

public class GetUserAccountsQuery : IRequest<GetUserAccountsQueryResponse>
{
    public string UserRef { get; set; }
}