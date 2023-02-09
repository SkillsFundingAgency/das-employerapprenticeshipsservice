namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByIdQuery : IRequest<GetEmployerAccountByIdResponse>
{
    public long AccountId { get; set; }
    public string UserId { get; set; }
}