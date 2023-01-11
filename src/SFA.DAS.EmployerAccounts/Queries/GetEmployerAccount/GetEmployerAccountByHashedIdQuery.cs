namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByHashedIdQuery : IAsyncRequest<GetEmployerAccountByHashedIdResponse>
{
    public string HashedAccountId { get; set; }
    public string UserId { get; set; }
}