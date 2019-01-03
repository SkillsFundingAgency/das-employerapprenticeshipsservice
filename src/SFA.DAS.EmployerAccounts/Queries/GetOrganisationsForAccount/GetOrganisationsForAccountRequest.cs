using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsForAccount
{
    public class GetOrganisationsForAccountRequest : IAsyncRequest<GetOrganisationsForAccountResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
    }
}
