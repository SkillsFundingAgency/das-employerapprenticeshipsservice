namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByHashedIdQuery : IRequest<GetEmployerAccountByHashedIdResponse>
{
    public long AccountId { get; set; }
    public string UserId { get; set; }
}