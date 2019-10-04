using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountOwner
{
    public class GetAccountOwnerQuery : IAsyncRequest<GetAccountOwnerResponse>
    {
        public string HashedAccountId { get; set; }
    }
}
