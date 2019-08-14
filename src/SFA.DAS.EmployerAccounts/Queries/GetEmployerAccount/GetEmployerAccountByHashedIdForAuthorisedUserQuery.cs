using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdForAuthorisedUserQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
    }
}
