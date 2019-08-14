using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public string HashedAccountId { get; set; }
    }
}